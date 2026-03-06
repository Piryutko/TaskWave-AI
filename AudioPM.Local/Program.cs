using System.Diagnostics;
using AudioPM.Local.Models;
using AudioPM.Local.Services;
using AudioPM.Local.UI;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using static AudioPM.Local.UI.ScreenRenderer;

// ─── DI / HTTP setup ─────────────────────────────────────────────────────────

var services = new ServiceCollection();
services.AddHttpClient<OllamaService>(client => { client.Timeout = TimeSpan.FromMinutes(10); });
services.AddSingleton<WhisperService>();

var provider      = services.BuildServiceProvider();
var whisperService = provider.GetRequiredService<WhisperService>();
var ollamaService  = provider.GetRequiredService<OllamaService>();

// ─── Screen 1: Setup Check ───────────────────────────────────────────────────

AnsiConsole.Clear();

bool ollamaAvailable = false;

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .HideCompleted(false)
    .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new SpinnerColumn(Spinner.Known.Dots))
    .StartAsync(async ctx =>
    {
        var bannerTask = ctx.AddTask("[deepskyblue1]Запуск AudioPM Local[/]");
        bannerTask.IsIndeterminate = true;

        var scanTask = ctx.AddTask("[grey]Поиск Whisper моделей на диске...[/]");
        scanTask.IsIndeterminate = true;
        var foundModel = whisperService.FindAnyPresentModel();
        scanTask.Description = foundModel is not null
            ? $"[green]✓ Найдена модель: {foundModel.DisplayName} ({foundModel.Size})[/]"
            : "[yellow]⚠ Whisper модели не найдены[/]";
        scanTask.Value = 100;
        scanTask.StopTask();

        var ollamaTask = ctx.AddTask("[grey]Подключение к Ollama...[/]");
        ollamaTask.IsIndeterminate = true;
        ollamaAvailable = await ollamaService.IsAvailableAsync();
        ollamaTask.Description = ollamaAvailable
            ? "[green]✓ Ollama доступна на localhost:11434[/]"
            : "[red]✗ Ollama не отвечает[/]";
        ollamaTask.Value = 100;
        ollamaTask.StopTask();

        bannerTask.Value = 100;
        bannerTask.StopTask();
    });

// ─── Whisper model selection ─────────────────────────────────────────────────

// Auto-select any already-downloaded model, or let user pick + download
var existingModel = whisperService.FindAnyPresentModel();

WhisperModelInfo chosenWhisperModel;

if (existingModel is not null)
{
    // At least one model present — let user confirm or switch
    chosenWhisperModel = ScreenRenderer.AskForWhisperModel(current: existingModel);
}
else
{
    // No model found — must pick one to download
    chosenWhisperModel = ScreenRenderer.AskForWhisperModel();
}

await whisperService.SetModelAsync(chosenWhisperModel);

// Download if the chosen model isn't on disk yet
if (!whisperService.IsModelPresent())
{
    if (ScreenRenderer.AskToDownloadModel(chosenWhisperModel, whisperService.ModelPath))
    {
        var downloaded = await ScreenRenderer.ShowDownloadModelScreen(
            (progress, ct) => whisperService.DownloadModelAsync(progress, ct),
            whisperService.ModelPath);

        if (!downloaded) { await whisperService.DisposeAsync(); return 1; }
    }
    else
    {
        ScreenRenderer.ShowSetupError(
            ollamaDown: false, modelMissing: true,
            ollamaInstructions: string.Empty,
            modelInstructions: WhisperService.GetDownloadInstructions(chosenWhisperModel, whisperService.ModelPath));
        await whisperService.DisposeAsync();
        return 1;
    }
}

// ─── Ollama availability check ───────────────────────────────────────────────

if (!ollamaAvailable)
{
    ScreenRenderer.ShowSetupError(
        ollamaDown: true, modelMissing: false,
        ollamaInstructions: OllamaService.GetOllamaSetupInstructions(),
        modelInstructions: string.Empty);
    await whisperService.DisposeAsync();
    return 1;
}

// ─── Ollama model selection ───────────────────────────────────────────────────

var availableModels = await ollamaService.GetAvailableModelsAsync();
var selectedLlm     = ScreenRenderer.AskForOllamaModel(availableModels, preferred: "llama3");

if (selectedLlm is null) { await whisperService.DisposeAsync(); return 1; }
ollamaService.ModelName = selectedLlm;

ScreenRenderer.ShowSetupOk(ollamaService.ModelName);

// ─── Main loop ───────────────────────────────────────────────────────────────

int sessionsCompleted = 0;

while (true)
{
    var menuChoice = ScreenRenderer.ShowMainMenu(
        ollamaService.ModelName,
        whisperService.SelectedModel.DisplayName,
        sessionsCompleted);

    // ── Exit ─────────────────────────────────────────────────────────────────
    if (menuChoice == MainMenuChoice.Exit)
        break;

    // ── Change Ollama model ───────────────────────────────────────────────────
    if (menuChoice == MainMenuChoice.ChangeModel)
    {
        var models   = await ollamaService.GetAvailableModelsAsync();
        var newModel = ScreenRenderer.AskForOllamaModel(models, preferred: ollamaService.ModelName);
        if (newModel is not null) ollamaService.ModelName = newModel;
        continue;
    }

    // ── Change Whisper model ───────────────────────────────────────────────────
    if (menuChoice == MainMenuChoice.ChangeWhisperModel)
    {
        var newWhisper = ScreenRenderer.AskForWhisperModel(current: whisperService.SelectedModel);
        await whisperService.SetModelAsync(newWhisper);

        if (!whisperService.IsModelPresent())
        {
            if (ScreenRenderer.AskToDownloadModel(newWhisper, whisperService.ModelPath))
            {
                await ScreenRenderer.ShowDownloadModelScreen(
                    (p, ct) => whisperService.DownloadModelAsync(p, ct),
                    whisperService.ModelPath);
            }
        }
        continue;
    }

    // ── Analyze meeting ───────────────────────────────────────────────────────

    var audioFilePath = ScreenRenderer.AskForAudioFile();
    var mode          = ScreenRenderer.AskForMode();
    var includeDodAc  = ScreenRenderer.AskForDodAc();
    var appState      = new AppState(audioFilePath, mode, includeDodAc);

    ScreenRenderer.ShowProcessingScreen(Path.GetFileName(audioFilePath), mode, includeDodAc);

    string transcribedText = string.Empty;
    string analysisResult  = string.Empty;
    var transcriptionSw = Stopwatch.StartNew();
    var analysisSw      = new Stopwatch();

    try
    {
        await AnsiConsole.Progress()
            .AutoRefresh(true)
            .HideCompleted(false)
            .Columns(
                new TaskDescriptionColumn { Alignment = Justify.Left },
                new ProgressBarColumn(),
                new SpinnerColumn(Spinner.Known.Star))
            .StartAsync(async ctx =>
            {
                // Step 1 — Whisper init
                var initTask = ctx.AddTask($"[bold]Инициализация Whisper ({whisperService.SelectedModel.DisplayName})...[/]");
                initTask.IsIndeterminate = true;
                await whisperService.InitializeAsync(
                    new Progress<string>(m => initTask.Description = $"[deepskyblue1]{Markup.Escape(m)}[/]"));
                initTask.Value = 100;
                initTask.Description = $"[green]✓ Whisper {whisperService.SelectedModel.DisplayName} инициализирован[/]";
                initTask.StopTask();

                // Step 2 — Transcription
                var transcribeTask = ctx.AddTask("[bold]Локальная расшифровка аудио...[/]");
                transcribeTask.IsIndeterminate = true;
                transcribedText = await whisperService.TranscribeAsync(audioFilePath,
                    new Progress<string>(m => transcribeTask.Description = $"[deepskyblue1]{Markup.Escape(m)}[/]"));
                transcriptionSw.Stop();
                transcribeTask.Value = 100;
                transcribeTask.Description = $"[green]✓ Расшифровка завершена ({transcriptionSw.Elapsed:mm\\:ss})[/]";
                transcribeTask.StopTask();

                // Step 2.5 — AI Corrector
                var refineSw   = Stopwatch.StartNew();
                var refineTask = ctx.AddTask("[bold]ИИ исправляет технические ошибки...[/]");
                refineTask.IsIndeterminate = true;
                var refinedText = await ollamaService.RefineTranscriptionAsync(transcribedText,
                    new Progress<string>(m => refineTask.Description = $"[yellow]{Markup.Escape(m)}[/]"));
                refineSw.Stop();
                refineTask.Value = 100;
                refineTask.Description = $"[green]✓ Текст очищен ({refineSw.Elapsed:mm\\:ss})[/]";
                refineTask.StopTask();

                // Step 3 — Ollama generation
                analysisSw.Start();
                var ollamaTask = ctx.AddTask("[bold]Генерация проектной документации...[/]");
                ollamaTask.IsIndeterminate = true;
                analysisResult = await ollamaService.GenerateDocumentationAsync(
                    refinedText, mode, includeDodAc,
                    new Progress<string>(m => ollamaTask.Description = $"[mediumpurple1]{Markup.Escape(m)}[/]"));
                analysisSw.Stop();
                ollamaTask.Value = 100;
                ollamaTask.Description = $"[green]✓ Документация сгенерирована ({analysisSw.Elapsed:mm\\:ss})[/]";
                ollamaTask.StopTask();
            });
    }
    catch (Exception ex)
    {
        ScreenRenderer.ShowFatalError("Ошибка во время обработки", ex);
        continue; // return to menu
    }

    // ── Save output ───────────────────────────────────────────────────────────

    var outputDir      = Path.GetDirectoryName(audioFilePath) ?? AppContext.BaseDirectory;
    var outputFileName = $"AudioPM_{Path.GetFileNameWithoutExtension(audioFilePath)}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
    var outputPath     = Path.Combine(outputDir, outputFileName);
    var fileInfo       = new FileInfo(audioFilePath);

    var header = $"""
        === AudioPM LOCAL — Документ проектной документации ===
        Дата создания  : {DateTime.Now:dd.MM.yyyy HH:mm:ss}
        Исходный файл  : {fileInfo.Name}
        Whisper модель : {whisperService.SelectedModel.DisplayName} ({whisperService.SelectedModel.FileName})
        Модель LLM     : {ollamaService.ModelName}
        Режим          : {(mode == AnalysisMode.EpicsDecomposition ? "Эпики и декомпозиция" : "Простой список задач")}
        DoD/AC         : {(includeDodAc ? "Включены" : "Отключены")}
        Время расшифр. : {transcriptionSw.Elapsed:mm\:ss\.ff}
        Время анализа  : {analysisSw.Elapsed:mm\:ss\.ff}
        ==================================================

        """;

    await File.WriteAllTextAsync(outputPath, header + analysisResult);

    var result = new ProcessingResult(
        TranscribedText:       transcribedText,
        AnalysisMarkdown:      analysisResult,
        OutputFilePath:        outputPath,
        AudioFileSizeBytes:    fileInfo.Length,
        TranscriptionDuration: transcriptionSw.Elapsed,
        AnalysisDuration:      analysisSw.Elapsed);

    ScreenRenderer.ShowSuccessScreen(result, appState);
    sessionsCompleted++;
}

// ─── Clean exit ──────────────────────────────────────────────────────────────

AnsiConsole.Clear();
AnsiConsole.Write(new FigletText("ПОКА!").Centered().Color(Color.DeepSkyBlue1));
AnsiConsole.MarkupLine($"\n[grey]Завершено сессий: [white]{sessionsCompleted}[/]. До встречи![/]\n");

await whisperService.DisposeAsync();
return 0;
