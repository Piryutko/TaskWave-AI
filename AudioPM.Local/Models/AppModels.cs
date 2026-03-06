namespace AudioPM.Local.Models;

public enum AnalysisMode
{
    EpicsDecomposition,
    SimpleTaskList
}

public record AppState(
    string AudioFilePath,
    AnalysisMode Mode,
    bool IncludeDodAc
);

public record ProcessingResult(
    string TranscribedText,
    string AnalysisMarkdown,
    string OutputFilePath,
    long AudioFileSizeBytes,
    TimeSpan TranscriptionDuration,
    TimeSpan AnalysisDuration
);

// ─── Whisper model catalog ────────────────────────────────────────────────────

public record WhisperModelInfo(
    string DisplayName,
    string FileName,
    string Size,
    string Description
)
{
    public string DownloadUrl =>
        $"https://huggingface.co/ggerganov/whisper.cpp/resolve/main/{FileName}";

    /// <summary>All supported models, ordered from fastest to most accurate.</summary>
    public static readonly WhisperModelInfo[] All =
    [
        new("Base",   "ggml-base.bin",   "~145 МБ", "Быстрая, подходит для тестов"),
        new("Small",  "ggml-small.bin",  "~480 МБ", "Баланс скорости и точности  ★ Рекомендуется"),
        new("Medium", "ggml-medium.bin", "~1.5 ГБ", "Максимальная точность, технический контекст"),
    ];

    public static WhisperModelInfo Default => All[1]; // Small
}

