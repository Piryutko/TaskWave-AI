using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Whisper.net;
using Whisper.net.Ggml;
using AudioPM.Local.Models;

namespace AudioPM.Local.Services;

public sealed class WhisperService : IAsyncDisposable
{
    private WhisperModelInfo _selectedModel = WhisperModelInfo.Default;
    private WhisperFactory? _factory;
    private WhisperProcessor? _processor;

    /// <summary>The currently active Whisper model.</summary>
    public WhisperModelInfo SelectedModel => _selectedModel;

    /// <summary>Full path to the active model file.</summary>
    public string ModelPath => Path.Combine(AppContext.BaseDirectory, _selectedModel.FileName);

    /// <summary>True if the currently selected model file exists on disk.</summary>
    public bool IsModelPresent() => File.Exists(ModelPath);

    /// <summary>
    /// Returns the first model that is present on disk, or null if none found.
    /// Used at startup to auto-select an existing model.
    /// </summary>
    public WhisperModelInfo? FindAnyPresentModel() =>
        WhisperModelInfo.All.FirstOrDefault(m =>
            File.Exists(Path.Combine(AppContext.BaseDirectory, m.FileName)));

    /// <summary>Changes the active model. Disposes the current processor if built.</summary>
    public async Task SetModelAsync(WhisperModelInfo model)
    {
        if (_processor is not null) { await _processor.DisposeAsync(); _processor = null; }
        _factory?.Dispose(); _factory = null;
        _selectedModel = model;
    }

    public Task InitializeAsync(IProgress<string>? progress = null)
    {
        progress?.Report("Загрузка модели Whisper из файла...");

        _factory = WhisperFactory.FromPath(ModelPath);

        progress?.Report("Создание процессора транскрипции...");

        _processor = _factory.CreateBuilder()
            .WithLanguage("auto")
            .WithPrompt("Daily, API, фронтенд, Jira, векторизация, документация, баг, бэкенд, сервис, спринт, эпик, дедлайн, деплой, пулл-реквест, сторипоинты")
            .Build();

        return Task.CompletedTask;
    }

    public async Task<string> TranscribeAsync(
        string audioFilePath,
        IProgress<string>? progress = null,
        CancellationToken ct = default)
    {
        if (_processor is null)
            throw new InvalidOperationException("WhisperService не инициализирован. Вызовите InitializeAsync() сначала.");

        // Whisper.net requires a 16 kHz mono 16-bit PCM WAV stream.
        // Convert the source file first (handles MP3, M4A, OGG, FLAC, WAV, etc.)
        progress?.Report($"Конвертация аудио в WAV 16 кГц: {Path.GetFileName(audioFilePath)}");
        var tempWav = await ConvertToWhisperWavAsync(audioFilePath, ct);

        try
        {
            progress?.Report("Запуск расшифровки...");

            var segments = new List<string>();

            await using var fileStream = File.OpenRead(tempWav);

            await foreach (var segment in _processor.ProcessAsync(fileStream, ct))
            {
                segments.Add(segment.Text);
                var preview = segment.Text.Trim();
                if (preview.Length > 40) preview = preview[..40] + "...";
                progress?.Report($"[{segment.Start:mm\\:ss} → {segment.End:mm\\:ss}]: {preview}");
            }

            return string.Join(" ", segments).Trim();
        }
        finally
        {
            // Always remove the temp WAV, even if transcription throws
            if (File.Exists(tempWav))
                File.Delete(tempWav);
        }
    }

    /// <summary>
    /// Converts any audio format supported by Windows Media Foundation
    /// (MP3, M4A, OGG, WMA, FLAC, WAV…) to a temporary 16 kHz / 16-bit / mono WAV file
    /// that Whisper.net can parse.
    /// </summary>
    private static Task<string> ConvertToWhisperWavAsync(
        string inputPath,
        CancellationToken ct)
    {
        // Run the blocking NAudio conversion on the thread pool so we don't block the main thread
        return Task.Run(() =>
        {
            ct.ThrowIfCancellationRequested();

            var tempPath = Path.Combine(Path.GetTempPath(),
                $"audiopm_{Guid.NewGuid():N}.wav");

            // MediaFoundationReader handles: MP3, AAC/M4A, WMA, FLAC, OGG (with codec),
            // and already-valid WAV files of any sample rate/channel count.
            using var reader = new MediaFoundationReader(inputPath);

            // Target: 16 000 Hz, 16-bit, 1 channel — the exact format Whisper expects
            var targetFormat = new WaveFormat(16_000, 16, 1);
            using var resampler = new MediaFoundationResampler(reader, targetFormat)
            {
                ResamplerQuality = 60  // 1 (fast) … 60 (best quality)
            };

            WaveFileWriter.CreateWaveFile(tempPath, resampler);

            ct.ThrowIfCancellationRequested();
            return tempPath;
        }, ct);
    }

    private const string ModelDownloadUrl =
        "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-base.bin";

    /// <summary>
    /// Downloads ggml-base.bin from HuggingFace with streaming progress.
    /// Writes to a .tmp file first — moved to final path only on success.
    /// Progress reports (downloadedBytes, totalBytes). totalBytes = -1 if unknown.
    /// </summary>
    public async Task DownloadModelAsync(
        IProgress<(long Downloaded, long Total)>? progress = null,
        CancellationToken ct = default)
    {
        var tempPath = ModelPath + ".tmp";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "AudioPM.Local/1.0");
        httpClient.Timeout = TimeSpan.FromMinutes(60); // medium = 1.5 GB

        using var response = await httpClient.GetAsync(
            _selectedModel.DownloadUrl,
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;

        await using var remoteStream = await response.Content.ReadAsStreamAsync(ct);
        await using var fileStream = new FileStream(
            tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);

        var buffer = new byte[81920];
        long downloadedBytes = 0L;
        int bytesRead;

        while ((bytesRead = await remoteStream.ReadAsync(buffer, ct)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
            downloadedBytes += bytesRead;
            progress?.Report((downloadedBytes, totalBytes));
        }

        await fileStream.FlushAsync(ct);
        fileStream.Close();

        if (File.Exists(ModelPath)) File.Delete(ModelPath);
        File.Move(tempPath, ModelPath);
    }

    public static string GetDownloadInstructions(WhisperModelInfo model, string modelPath) =>
        $"""
        Модель {model.FileName} не найдена.

        Ожидаемый путь: {modelPath}

        Скачайте вручную:
           curl -L "{model.DownloadUrl}" -o "{modelPath}"
        """;



    public async ValueTask DisposeAsync()
    {
        if (_processor is not null)
            await _processor.DisposeAsync();

        _factory?.Dispose();
    }
}
