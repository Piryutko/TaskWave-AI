using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TaskWave.Api.Models;

namespace TaskWave.Api.Services;

public sealed class OllamaService(HttpClient httpClient)
{
    private const string OllamaBaseUrl = "http://localhost:11434";

    public string ModelName { get; set; } = string.Empty;

    private const string SystemPrompt =
        """
        Ты — локальный Senior Project Manager. Проанализируй предоставленный текст встречи.
        Твой ответ должен быть на РУССКОМ языке.
        Структурируй его как документ для сохранения в .txt файл.
        Используй заголовки, списки и разделители '==='.
        Если выбран режим 'Эпики', делай иерархию. Если 'Задачи' — плоский список.
        Если включена опция DoD/AC — добавь их технически точно для каждого пункта.
        """;

    private const string CorrectorSystemPrompt =
        """
        Ты — технический редактор русскоязычных транскрипций митингов.
        Исправь ошибки автоматического распознавания речи.
        Заменяй фонетически похожие ошибки на правильные IT-термины.
        НЕ добавляй новый контент — только исправляй.
        Верни ТОЛЬКО исправленный текст без комментариев.
        """;

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            var res = await httpClient.GetAsync($"{OllamaBaseUrl}/api/tags", ct);
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<string>> GetAvailableModelsAsync(CancellationToken ct = default)
    {
        try
        {
            var res = await httpClient.GetFromJsonAsync<OllamaTagsResponse>($"{OllamaBaseUrl}/api/tags", ct);
            return res?.Models.Select(m => m.Name).OrderBy(n => n).ToList() ?? [];
        }
        catch { return []; }
    }

    // ── Step A: Corrector ──────────────────────────────────────────────────────

    public async Task<string> RefineTranscriptionAsync(
        string rawText,
        IProgress<string>? progress = null,
        CancellationToken ct = default)
    {
        progress?.Report("ИИ исправляет технические ошибки...");
        var sb = new StringBuilder();
        await foreach (var token in StreamOllamaAsync(rawText, CorrectorSystemPrompt, ct))
        {
            sb.Append(token);
            if (sb.Length % 200 == 0)
                progress?.Report($"Коррекция... {sb.Length} символов");
        }
        var result = sb.ToString().Trim();
        return string.IsNullOrWhiteSpace(result) ? rawText : result;
    }

    // ── Step B: Analyst ────────────────────────────────────────────────────────

    public async Task<string> GenerateDocumentationAsync(
        string transcribedText,
        string mode,
        bool includeDodAc,
        IProgress<string>? progress = null,
        CancellationToken ct = default)
    {
        var modeInstruction = mode == "epics"
            ? "РЕЖИМ: Эпики и декомпозиция. Создай иерархическую структуру: Эпик → Задача → Подзадача."
            : "РЕЖИМ: Простой список задач. Создай плоский нумерованный список задач.";

        var dodInstruction = includeDodAc
            ? "ОПЦИЯ ВКЛЮЧЕНА: Для каждого пункта добавь секции 'Критерии готовности (DoD):' и 'Критерии приемки (AC):'."
            : string.Empty;

        var userPrompt =
            $"""
            {modeInstruction}
            {dodInstruction}

            === ТЕКСТ ВСТРЕЧИ ===
            {transcribedText}
            === КОНЕЦ ТЕКСТА ===

            Сгенерируй полный документ проектной документации.
            """;

        progress?.Report($"Генерация документации...");
        var sb = new StringBuilder();
        int tokenCount = 0;

        await foreach (var token in StreamOllamaAsync(userPrompt, SystemPrompt, ct))
        {
            sb.Append(token);
            tokenCount++;
            if (tokenCount % 15 == 0)
                progress?.Report($"Генерация... {tokenCount} токенов");
        }

        var result = sb.ToString().Trim();
        if (string.IsNullOrWhiteSpace(result))
            throw new InvalidOperationException("Модель вернула пустой ответ.");

        progress?.Report($"Готово — {tokenCount} токенов.");
        return result;
    }

    // ── Shared streaming helper ────────────────────────────────────────────────

    private async IAsyncEnumerable<string> StreamOllamaAsync(
        string prompt,
        string systemPrompt,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(new OllamaRequest(
            Model:  ModelName,
            Prompt: prompt,
            System: systemPrompt,
            Stream: true));

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var req = new HttpRequestMessage(HttpMethod.Post, $"{OllamaBaseUrl}/api/generate") { Content = content };

        using var resp = await httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        resp.EnsureSuccessStatusCode();

        await using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        while (!reader.EndOfStream)
        {
            ct.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line)) continue;

            OllamaResponse? chunk;
            try { chunk = JsonSerializer.Deserialize<OllamaResponse>(line); }
            catch (JsonException) { continue; }

            if (chunk is null) continue;
            if (!string.IsNullOrEmpty(chunk.Error)) throw new InvalidOperationException(chunk.Error);

            yield return chunk.Response;

            if (chunk.Done) break;
        }
    }
}
