using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Whisper.net;
using TaskWave.Api.Models;

namespace TaskWave.Api.Services;

public sealed class WhisperService : IAsyncDisposable
{
    private WhisperModelInfo _selectedModel = WhisperModelInfo.Default;
    private WhisperFactory? _factory;
    private WhisperProcessor? _processor;

    public WhisperModelInfo SelectedModel => _selectedModel;

    // Search models in multiple locations:
    // 1. Alongside this binary (bin/Debug/net8.0/)
    // 2. AudioPM.Local bin output (sibling project)
    private static readonly string[] ModelSearchDirs =
    [
        AppContext.BaseDirectory,
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AudioPM.Local", "bin", "Debug", "net8.0"),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AudioPM.Local", "bin", "Release", "net8.0"),
    ];

    public string ModelPath => FindModelFile(_selectedModel.FileName)
        ?? Path.Combine(AppContext.BaseDirectory, _selectedModel.FileName);

    private static string? FindModelFile(string fileName) =>
        ModelSearchDirs
            .Select(d => Path.GetFullPath(Path.Combine(d, fileName)))
            .FirstOrDefault(File.Exists);

    public bool IsModelPresent() => FindModelFile(_selectedModel.FileName) is not null;

    public WhisperModelInfo? FindAnyPresentModel() =>
        WhisperModelInfo.All.FirstOrDefault(m => FindModelFile(m.FileName) is not null);

    public async Task SetModelAsync(WhisperModelInfo model)
    {
        if (_processor is not null) { await _processor.DisposeAsync(); _processor = null; }
        _factory?.Dispose(); _factory = null;
        _selectedModel = model;
    }

    public Task InitializeAsync(IProgress<string>? progress = null)
    {
        progress?.Report("Загрузка AI-модели распознавания...");
        _factory = WhisperFactory.FromPath(ModelPath);
        progress?.Report("Создание процессора...");
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
            throw new InvalidOperationException("WhisperService не инициализирован.");

        progress?.Report($"Конвертация аудио: {Path.GetFileName(audioFilePath)}");
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
                progress?.Report($"[{segment.Start:mm\\:ss}]: {preview}");
            }

            return string.Join(" ", segments).Trim();
        }
        finally
        {
            if (File.Exists(tempWav)) File.Delete(tempWav);
        }
    }

    private static Task<string> ConvertToWhisperWavAsync(string inputPath, CancellationToken ct) =>
        Task.Run(() =>
        {
            ct.ThrowIfCancellationRequested();
            var tempPath = Path.Combine(Path.GetTempPath(), $"taskwave_{Guid.NewGuid():N}.wav");
            using var reader = new MediaFoundationReader(inputPath);
            var targetFormat = new WaveFormat(16_000, 16, 1);
            using var resampler = new MediaFoundationResampler(reader, targetFormat) { ResamplerQuality = 60 };
            WaveFileWriter.CreateWaveFile(tempPath, resampler);
            ct.ThrowIfCancellationRequested();
            return tempPath;
        }, ct);

    public async ValueTask DisposeAsync()
    {
        if (_processor is not null) await _processor.DisposeAsync();
        _factory?.Dispose();
    }
}
