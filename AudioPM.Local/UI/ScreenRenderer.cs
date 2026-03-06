using Spectre.Console;
using AudioPM.Local.Models;

namespace AudioPM.Local.UI;

/// <summary>
/// Centralised renderer for all application screens.
/// Every public method clears the console first to simulate a "new screen" effect.
/// </summary>
public static class ScreenRenderer
{
    // ─── Brand banner ────────────────────────────────────────────────────────

    private static void RenderBanner(string subtitle = "")
    {
        AnsiConsole.Write(
            new FigletText("AUDIO PM")
                .Centered()
                .Color(Color.DeepSkyBlue1));

        AnsiConsole.Write(
            new FigletText("[LOCAL]")
                .Centered()
                .Color(Color.MediumPurple1));

        if (!string.IsNullOrWhiteSpace(subtitle))
        {
            AnsiConsole.Write(
                new Rule($"[grey]{subtitle}[/]")
                    .RuleStyle("grey27")
                    .Centered());
        }

        AnsiConsole.WriteLine();
    }

    // ─── Screen 1 — Setup Check ──────────────────────────────────────────────

    public static void ShowSetupError(bool ollamaDown, bool modelMissing,
        string ollamaInstructions, string modelInstructions)
    {
        AnsiConsole.Clear();
        RenderBanner("Проверка окружения");

        if (ollamaDown)
        {
            AnsiConsole.Write(new Panel(
                    new Markup($"[bold red]✗ Ollama недоступна[/]\n\n[grey]{Markup.Escape(ollamaInstructions)}[/]"))
                .Header("[red]Ollama[/]")
                .BorderColor(Color.Red3)
                .Padding(1, 1));

            AnsiConsole.WriteLine();
        }

        if (modelMissing)
        {
            AnsiConsole.Write(new Panel(
                    new Markup($"[bold yellow]✗ Модель ggml-base.bin не найдена[/]\n\n[grey]{Markup.Escape(modelInstructions)}[/]"))
                .Header("[yellow]Whisper Model[/]")
                .BorderColor(Color.Yellow)
                .Padding(1, 1));

            AnsiConsole.WriteLine();
        }

        AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу для выхода...[/]");
        Console.ReadKey(true);
    }

    // ─── Model download prompt ────────────────────────────────────────────────

    // ─── Whisper model selection ──────────────────────────────────────────────

    public static WhisperModelInfo AskForWhisperModel(WhisperModelInfo? current = null)
    {
        AnsiConsole.Clear();
        RenderBanner("Выбор Whisper модели");

        AnsiConsole.MarkupLine("[grey]Модель влияет на точность расшифровки. Больше — лучше, но медленнее.[/]");
        AnsiConsole.WriteLine();

        // Build plain-text choice strings — NO square brackets (Spectre treats them as markup tags)
        var choices = WhisperModelInfo.All
            .Select(m => Markup.Escape(
                $"{m.Size,-10}  {m.DisplayName,-8}  {m.Description}" +
                (m == current ? "  (текущая)" : string.Empty)))
            .ToArray();

        var prompt = new SelectionPrompt<string>()
            .Title("[bold]Выберите модель Whisper:[/]")
            .PageSize(5)
            .HighlightStyle(new Style(foreground: Color.DeepSkyBlue1))
            .AddChoices(choices);

        var selected = AnsiConsole.Prompt(prompt);
        var idx = Array.IndexOf(choices, selected);
        var model = WhisperModelInfo.All[idx];

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[green]✓ Выбрано:[/] [bold]{model.DisplayName}[/] [grey]({model.Size})[/]");
        Thread.Sleep(700);
        return model;
    }

    /// <summary>Ask the user whether to auto-download the model. Returns true = yes.</summary>
    public static bool AskToDownloadModel(WhisperModelInfo model, string modelPath)
    {
        AnsiConsole.Clear();
        RenderBanner("Загрузка модели Whisper");

        AnsiConsole.Write(new Panel(
                new Markup(
                    $"[bold yellow]Модель {Markup.Escape(model.FileName)} не найдена.[/]\n\n" +
                    $"[grey]Путь установки:[/] [white]{Markup.Escape(modelPath)}[/]\n\n" +
                    $"[grey]Размер файла: [/][white]{Markup.Escape(model.Size)}[/] [grey]· Источник:[/] [link]https://huggingface.co/ggerganov/whisper.cpp[/]"))
            .Header("[yellow]Whisper Base Model[/]")
            .BorderColor(Color.Yellow)
            .Padding(1, 1));

        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Скачать модель автоматически?[/]")
                .HighlightStyle(new Style(foreground: Color.DeepSkyBlue1))
                .AddChoices(
                    $"⬇️   Да — скачать сейчас ({Markup.Escape(model.Size)})",
                    "✖   Нет — выйти и скачать вручную"
                ));

        return choice.StartsWith("⬇");
    }

    /// <summary>
    /// Shows the download screen and runs the actual download with an AnsiConsole progress bar.
    /// Returns true on success, false on error.
    /// </summary>
    public static async Task<bool> ShowDownloadModelScreen(
        Func<IProgress<(long Downloaded, long Total)>, CancellationToken, Task> downloadAction,
        string modelPath)
    {
        AnsiConsole.Clear();
        RenderBanner("Загрузка модели Whisper");

        AnsiConsole.MarkupLine("[bold deepskyblue1]⬇️  Скачивание ggml-base.bin с HuggingFace...[/]");
        AnsiConsole.WriteLine();

        bool success = false;
        Exception? error = null;

        var sw = System.Diagnostics.Stopwatch.StartNew();

        await AnsiConsole.Progress()
            .AutoRefresh(true)
            .HideCompleted(false)
            .Columns(
                new TaskDescriptionColumn { Alignment = Justify.Left },
                new ProgressBarColumn(),
                new PercentageColumn(),
                new TransferSpeedColumn(),
                new RemainingTimeColumn())
            .StartAsync(async ctx =>
            {
                var dlTask = ctx.AddTask("[deepskyblue1]ggml-base.bin[/]", maxValue: 100);

                var progress = new Progress<(long Downloaded, long Total)>(report =>
                {
                    var (downloaded, total) = report;

                    if (total > 0)
                        dlTask.Value = downloaded * 100.0 / total;
                    else
                        dlTask.IsIndeterminate = true;

                    // Update description with MB
                    var mbDone = downloaded / (1024.0 * 1024);
                    var mbTotal = total > 0 ? $"/ {total / (1024.0 * 1024):F1} МБ" : string.Empty;
                    dlTask.Description = $"[deepskyblue1]ggml-base.bin[/]  [grey]{mbDone:F1} МБ {mbTotal}[/]";
                });

                try
                {
                    await downloadAction(progress, CancellationToken.None);
                    dlTask.Value = 100;
                    dlTask.Description = "[green]✓ ggml-base.bin — скачан[/]";
                    dlTask.StopTask();
                    success = true;
                }
                catch (Exception ex)
                {
                    error = ex;
                    dlTask.Description = "[red]✗ Ошибка загрузки[/]";
                    dlTask.StopTask();
                }
            });

        sw.Stop();
        AnsiConsole.WriteLine();

        if (success)
        {
            var fileSize = new FileInfo(modelPath).Length;
            AnsiConsole.Write(new Panel(
                    new Markup(
                        $"[bold green]✓ Модель успешно загружена![/]\n\n" +
                        $"[grey]Размер:[/] [white]{fileSize / (1024.0 * 1024):F1} МБ[/]\n" +
                        $"[grey]Время:[/]  [white]{sw.Elapsed:mm\\:ss}[/]\n" +
                        $"[grey]Путь:[/]   [white]{Markup.Escape(modelPath)}[/]"))
                .Header("[green]Загрузка завершена[/]")
                .BorderColor(Color.Green3)
                .Padding(1, 1));

            AnsiConsole.WriteLine();
            Thread.Sleep(1500);
        }
        else
        {
            AnsiConsole.Write(new Panel(
                    new Markup(
                        $"[bold red]✗ Не удалось скачать модель[/]\n\n" +
                        $"[grey]{Markup.Escape(error?.Message ?? "Неизвестная ошибка")}[/]\n\n" +
                        "[grey]Попробуйте скачать вручную:\n" +
                        "  curl -L https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-base.bin\n" +
                        $"       -o \"{Markup.Escape(modelPath)}\"[/]"))
                .Header("[red]Ошибка[/]")
                .BorderColor(Color.Red3)
                .Padding(1, 1));

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу для выхода...[/]");
            Console.ReadKey(true);
        }

        return success;
    }

    // ─── Ollama model selection ───────────────────────────────────────────

    /// <summary>
    /// Lets the user pick which Ollama model to use.
    /// Returns null if no models are available.
    /// </summary>
    public static string? AskForOllamaModel(List<string> availableModels, string preferred = "llama3")
    {
        AnsiConsole.Clear();
        RenderBanner("Выбор модели Ollama");

        if (availableModels.Count == 0)
        {
            AnsiConsole.Write(new Panel(
                    new Markup(
                        "[bold red]Модели Ollama не найдены.[/]\n\n" +
                        "[grey]Установите хотя бы одну модель, например:[/]\n" +
                        "  [white]ollama pull llama3.2[/]\n\n" +
                        "[grey]Затем повторите запуск AudioPM.Local.[/]"))
                .Header("[red]Нет моделей[/]")
                .BorderColor(Color.Red3)
                .Padding(1, 1));

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу для выхода...[/]");
            Console.ReadKey(true);
            return null;
        }

        // Pre-select preferred model if available, otherwise first in list
        var defaultChoice = availableModels.FirstOrDefault(m =>
            m.StartsWith(preferred, StringComparison.OrdinalIgnoreCase))
            ?? availableModels[0];

        AnsiConsole.MarkupLine($"[grey]Найдено моделей: [white]{availableModels.Count}[/][/]");
        AnsiConsole.WriteLine();

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Выберите модель для генерации документации:[/]")
                .PageSize(10)
                .HighlightStyle(new Style(foreground: Color.MediumPurple1))
                .AddChoices(availableModels));

        AnsiConsole.MarkupLine($"[green]✓ Выбрана:[/] [bold mediumpurple1]{Markup.Escape(selected)}[/]");
        Thread.Sleep(700);
        return selected;
    }

    public static void ShowSetupOk(string selectedModel)
    {
        AnsiConsole.Clear();
        RenderBanner("Проверка окружения");

        AnsiConsole.Write(new Panel(
                new Markup(
                    "[bold green]✓ Ollama запущена и доступна[/]\n" +
                    "[bold green]✓ Модель ggml-base.bin найдена[/]\n" +
                    $"[bold green]✓ Модель LLM:[/] [mediumpurple1]{Markup.Escape(selectedModel)}[/]"))
            .Header("[green]Всё готово[/]")
            .BorderColor(Color.Green3)
            .Padding(1, 1));

        AnsiConsole.WriteLine();
        Thread.Sleep(1200);
    }

    // ─── Main Menu ────────────────────────────────────────────────────────────

    public enum MainMenuChoice { Analyze, ChangeModel, ChangeWhisperModel, Exit }

    public static MainMenuChoice ShowMainMenu(string currentModel, string currentWhisperModel, int sessionsCompleted)
    {
        AnsiConsole.Clear();
        RenderBanner("Главное меню");

        var grid = new Grid().AddColumn(new GridColumn()).AddColumn(new GridColumn()).AddColumn(new GridColumn());
        grid.AddRow(
            $"[grey]Whisper:[/] [deepskyblue1]{Markup.Escape(currentWhisperModel)}[/]",
            $"[grey]LLM:[/] [mediumpurple1]{Markup.Escape(currentModel)}[/]",
            sessionsCompleted > 0
                ? $"[grey]Сессий:[/] [green]{sessionsCompleted}[/]"
                : "[grey]Сессий:[/] [grey]0[/]");
        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Что хотите сделать?[/]")
                .PageSize(7)
                .HighlightStyle(new Style(foreground: Color.DeepSkyBlue1))
                .AddChoices(
                    "  Анализировать встречу",
                    "  Сменить Ollama модель (LLM)",
                    "  Сменить Whisper модель (Расшифровка)",
                    "  Выйти"
                ));

        return choice.TrimStart()[0] switch
        {
            'А' => MainMenuChoice.Analyze,
            'С' when choice.Contains("Ollama") => MainMenuChoice.ChangeModel,
            'С' => MainMenuChoice.ChangeWhisperModel,
            _ => MainMenuChoice.Exit
        };
    }

    // ─── Screen 2 — File Selection ───────────────────────────────────────────

    public static string AskForAudioFile()
    {
        AnsiConsole.Clear();
        RenderBanner("Выбор аудиофайла");

        AnsiConsole.MarkupLine("[bold]Укажите путь к аудиофайлу[/] ([grey]WAV, MP3, OGG, FLAC, M4A...[/])");
        AnsiConsole.WriteLine();

        while (true)
        {
            var path = AnsiConsole.Ask<string>("[deepskyblue1]Путь к файлу:[/]").Trim().Trim('"');

            if (File.Exists(path))
            {
                AnsiConsole.MarkupLine($"[green]✓ Файл найден:[/] [bold]{Markup.Escape(Path.GetFileName(path))}[/]");
                Thread.Sleep(700);
                return path;
            }

            AnsiConsole.MarkupLine("[red]✗ Файл не найден. Проверьте путь и попробуйте снова.[/]");
        }
    }

    // ─── Screen 3 — Mode Selection ───────────────────────────────────────────

    public static AnalysisMode AskForMode()
    {
        AnsiConsole.Clear();
        RenderBanner("Режим анализа");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Выберите режим анализа встречи:[/]")
                .PageSize(5)
                .HighlightStyle(new Style(foreground: Color.DeepSkyBlue1))
                .AddChoices(
                    "🗂  Эпики и декомпозиция  — иерархия: Эпик → Задача → Подзадача",
                    "📋  Простой список задач  — плоский нумерованный список"
                ));

        var mode = choice.StartsWith("🗂")
            ? AnalysisMode.EpicsDecomposition
            : AnalysisMode.SimpleTaskList;

        AnsiConsole.MarkupLine($"[green]✓ Выбран режим:[/] [bold]{(mode == AnalysisMode.EpicsDecomposition ? "Эпики и декомпозиция" : "Простой список задач")}[/]");
        Thread.Sleep(700);
        return mode;
    }

    // ─── Screen 4 — DoD/AC Options ───────────────────────────────────────────

    public static bool AskForDodAc()
    {
        AnsiConsole.Clear();
        RenderBanner("Опции документа");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Добавить критерии готовности (DoD) и приёмки (AC)?[/]")
                .PageSize(4)
                .HighlightStyle(new Style(foreground: Color.MediumPurple1))
                .AddChoices(
                    "✅  Да — добавить DoD и AC для каждого пункта",
                    "❌  Нет — только задачи без критериев"
                ));

        var include = choice.StartsWith("✅");
        AnsiConsole.MarkupLine($"[green]✓[/] DoD/AC: [bold]{(include ? "Включены" : "Отключены")}[/]");
        Thread.Sleep(700);
        return include;
    }

    // ─── Screen 5 — Processing ───────────────────────────────────────────────

    public static void ShowProcessingScreen(string audioFileName, AnalysisMode mode, bool includeDodAc)
    {
        AnsiConsole.Clear();
        RenderBanner("Обработка");

        var settingsTable = new Table()
            .BorderColor(Color.Grey27)
            .AddColumn(new TableColumn("[grey]Параметр[/]").Width(25))
            .AddColumn(new TableColumn("[grey]Значение[/]"));

        settingsTable.AddRow("[grey]Файл[/]", $"[white]{Markup.Escape(audioFileName)}[/]");
        settingsTable.AddRow("[grey]Режим[/]",
            mode == AnalysisMode.EpicsDecomposition
                ? "[deepskyblue1]Эпики и декомпозиция[/]"
                : "[mediumpurple1]Простой список задач[/]");
        settingsTable.AddRow("[grey]DoD/AC[/]",
            includeDodAc ? "[green]Включены[/]" : "[red]Отключены[/]");

        AnsiConsole.Write(settingsTable);
        AnsiConsole.WriteLine();
    }

    // ─── Screen 6 — Success ──────────────────────────────────────────────────

    public static void ShowSuccessScreen(ProcessingResult result, AppState state)
    {
        AnsiConsole.Clear();
        RenderBanner("Готово!");

        AnsiConsole.Write(
            new FigletText("УСПЕХ!")
                .Centered()
                .Color(Color.Green3));

        AnsiConsole.WriteLine();

        var table = new Table()
            .Title("[bold green]Результаты обработки[/]")
            .BorderColor(Color.Green3)
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("[grey]Параметр[/]").Width(30))
            .AddColumn(new TableColumn("[grey]Значение[/]"));

        var fileInfo = new FileInfo(state.AudioFilePath);
        table.AddRow("[grey]Исходный файл[/]", $"[white]{Markup.Escape(fileInfo.Name)}[/]");
        table.AddRow("[grey]Размер файла[/]", $"[white]{FormatBytes(result.AudioFileSizeBytes)}[/]");
        table.AddRow("[grey]Режим анализа[/]",
            state.Mode == AnalysisMode.EpicsDecomposition
                ? "[deepskyblue1]Эпики и декомпозиция[/]"
                : "[mediumpurple1]Простой список задач[/]");
        table.AddRow("[grey]DoD/AC[/]",
            state.IncludeDodAc ? "[green]Включены[/]" : "[red]Отключены[/]");
        table.AddRow("[grey]Время расшифровки[/]",
            $"[white]{result.TranscriptionDuration:mm\\:ss\\.ff}[/]");
        table.AddRow("[grey]Время генерации[/]",
            $"[white]{result.AnalysisDuration:mm\\:ss\\.ff}[/]");
        table.AddRow("[grey]Файл результата[/]",
            $"[bold green]{Markup.Escape(result.OutputFilePath)}[/]");
        table.AddRow("[grey]Символов в документе[/]",
            $"[white]{result.AnalysisMarkdown.Length:N0}[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        AnsiConsole.Write(new Rule("[green]Готово! Возврат в главное меню...[/]").RuleStyle("green"));
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу для продолжения...[/]");
        Console.ReadKey(true);
    }

    // ─── Error screen ─────────────────────────────────────────────────────────

    public static void ShowFatalError(string message, Exception? ex = null)
    {
        AnsiConsole.Clear();
        RenderBanner("Критическая ошибка");

        AnsiConsole.WriteException(
            ex ?? new Exception(message),
            ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу для выхода...[/]");
        Console.ReadKey(true);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static string FormatBytes(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} Б",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} КБ",
        < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} МБ",
        _ => $"{bytes / (1024.0 * 1024 * 1024):F2} ГБ"
    };
}
