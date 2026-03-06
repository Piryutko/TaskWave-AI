namespace TaskWave.Api.Models;

public enum AnalysisMode { EpicsDecomposition, SimpleTaskList }

public record WhisperModelInfo(string DisplayName, string FileName, string Size, string Description)
{
    public string DownloadUrl =>
        $"https://huggingface.co/ggerganov/whisper.cpp/resolve/main/{FileName}";

    public static readonly WhisperModelInfo[] All =
    [
        new("Base",   "ggml-base.bin",   "~145 MB", "Fast, for testing"),
        new("Small",  "ggml-small.bin",  "~480 MB", "Balanced — recommended"),
        new("Medium", "ggml-medium.bin", "~1.5 GB", "Maximum accuracy"),
    ];

    public static WhisperModelInfo Default => All[1];
}

public record OllamaRequest(
    [property: System.Text.Json.Serialization.JsonPropertyName("model")]   string Model,
    [property: System.Text.Json.Serialization.JsonPropertyName("prompt")]  string Prompt,
    [property: System.Text.Json.Serialization.JsonPropertyName("system")]  string System,
    [property: System.Text.Json.Serialization.JsonPropertyName("stream")]  bool Stream
);

public record OllamaResponse(
    [property: System.Text.Json.Serialization.JsonPropertyName("response")] string Response,
    [property: System.Text.Json.Serialization.JsonPropertyName("done")]     bool Done,
    [property: System.Text.Json.Serialization.JsonPropertyName("error")]    string? Error
);

public record OllamaTagsResponse(
    [property: System.Text.Json.Serialization.JsonPropertyName("models")] List<OllamaModelInfo> Models
);

public record OllamaModelInfo(
    [property: System.Text.Json.Serialization.JsonPropertyName("name")] string Name
);
