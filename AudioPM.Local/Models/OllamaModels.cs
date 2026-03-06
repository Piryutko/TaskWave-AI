using System.Text.Json.Serialization;

namespace AudioPM.Local.Models;

public record OllamaRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("prompt")] string Prompt,
    [property: JsonPropertyName("system")] string System,
    [property: JsonPropertyName("stream")] bool Stream
);

public record OllamaResponse(
    [property: JsonPropertyName("response")] string Response,
    [property: JsonPropertyName("done")] bool Done,
    [property: JsonPropertyName("error")] string? Error
);

public record OllamaTagsResponse(
    [property: JsonPropertyName("models")] List<OllamaModelInfo> Models
);

public record OllamaModelInfo(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("size")] long Size
);

