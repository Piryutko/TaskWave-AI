using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AudioPM.Local.Models;

namespace AudioPM.Local.Services;

public sealed class OllamaService(HttpClient httpClient)
{
    private const string OllamaBaseUrl = "http://localhost:11434";

    /// <summary>Active model name. Set after user selects from available models.</summary>
    public string ModelName { get; set; } = "llama3";

    private const string SystemPrompt =
        """
        Ты — локальный Senior Project Manager. Проанализируй предоставленный текст встречи.
        Твой ответ должен быть на РУССКОМ языке.
        Структурируй его как документ для сохранения в .txt файл.
        Используй заголовки, списки и разделители '==='.
        Если выбран режим 'Эпики', делай иерархию. Если 'Задачи' — плоский список.
        Если включена опция DoD/AC — добавь их технически точно для каждого пункта.
        """;

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{OllamaBaseUrl}/api/tags", ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the list of model names installed in Ollama.
    /// Empty list if Ollama is unreachable.
    /// </summary>
    public async Task<List<string>> GetAvailableModelsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<OllamaTagsResponse>(
                $"{OllamaBaseUrl}/api/tags", ct);

            return response?.Models
                       .Select(m => m.Name)
                       .OrderBy(n => n)
                       .ToList()
                   ?? [];
        }
        catch
        {
            return [];
        }
    }

    // ─── Step A: Corrector ───────────────────────────────────────────────────

    private const string CorrectorSystemPrompt =
        """
        Ты — технический редактор русскоязычных транскрипций митингов.
        Твоя задача: исправить ошибки автоматического распознавания речи.
        
        Правила:
        - Заменяй фонетически похожие ошибки на правильные IT-термины:
          «пишки» → «API», «пи-эй-пи-ай» → «API»
          «фрондалисис» / «фронтадлис» / «фронталис» → «фронтенд-анализ»
          «делик» / «дейлик» / «дейло» → «Daily»
          «жира» / «джира» → «Jira»
          «стори поинт» / «сторипоинт» → «story points»
          «пул реквест» / «пул-реквест» → «pull request»
          «бекенд» / «бэк-энд» → «бэкенд»
          «деплоить» / «деплойт» → «деплоить»
          «векторизацыя» → «векторизация»
        - Исправляй грамматические ошибки и расставляй знаки препинания.
        - НЕ добавляй новый контент — только исправляй.
        - Верни ТОЛЬКО исправленный текст без комментариев.
        """;

    /// <summary>
    /// Step A — Corrector: fixes transcription errors before the main analysis.
    /// Returns the cleaned text ready for GenerateDocumentationAsync.
    /// </summary>
    public async Task<string> RefineTranscriptionAsync(
        string rawText,
        IProgress<string>? progress = null,
        CancellationToken ct = default)
    {
        progress?.Report("Подключение к Ollama (корректор)...");

        var json = System.Text.Json.JsonSerializer.Serialize(new OllamaRequest(
            Model:  ModelName,
            Prompt: rawText,
            System: CorrectorSystemPrompt,
            Stream: true));

        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{OllamaBaseUrl}/api/generate")
        {
            Content = content
        };

        using var httpResponse = await httpClient.SendAsync(
            httpRequest, HttpCompletionOption.ResponseHeadersRead, ct);

        httpResponse.EnsureSuccessStatusCode();

        await using var bodyStream = await httpResponse.Content.ReadAsStreamAsync(ct);
        using var reader = new System.IO.StreamReader(bodyStream, System.Text.Encoding.UTF8);

        var corrected = new System.Text.StringBuilder();
        int tokenCount = 0;

        progress?.Report("ИИ исправляет технические ошибки...");

        while (!reader.EndOfStream)
        {
            ct.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line)) continue;

            OllamaResponse? chunk;
            try { chunk = System.Text.Json.JsonSerializer.Deserialize<OllamaResponse>(line); }
            catch (System.Text.Json.JsonException) { continue; }

            if (chunk is null) continue;
            if (!string.IsNullOrEmpty(chunk.Error))
                throw new InvalidOperationException($"Ошибка Ollama (корректор): {chunk.Error}");

            corrected.Append(chunk.Response);
            tokenCount++;

            if (tokenCount % 20 == 0)
                progress?.Report($"ИИ исправляет текст... {tokenCount} токенов");

            if (chunk.Done) break;
        }

        var result = corrected.ToString().Trim();
        progress?.Report($"Текст очищен — {tokenCount} токенов.");

        // Fallback: if model returned nothing, pass original
        return string.IsNullOrWhiteSpace(result) ? rawText : result;
    }

    // ─── Step B: Analyst ─────────────────────────────────────────────────────

    public async Task<string> GenerateDocumentationAsync(
        string transcribedText,
        AnalysisMode mode,
        bool includeDodAc,
        IProgress<string>? progress = null,
        CancellationToken ct = default)
    {
        var modeInstruction = mode switch
        {
            AnalysisMode.EpicsDecomposition =>
                "РЕЖИМ: Эпики и декомпозиция. Создай иерархическую структуру: Эпик → Задача → Подзадача.",
            AnalysisMode.SimpleTaskList =>
                "РЕЖИМ: Простой список задач. Создай плоский нумерованный список задач.",
            _ => string.Empty
        };

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
            
            Сгенерируй полный документ проектной документации согласно инструкциям выше.
            """;

        // Build request with stream: true
        var request = new OllamaRequest(
            Model: ModelName,
            Prompt: userPrompt,
            System: SystemPrompt,
            Stream: true          // ← key change: streaming mode
        );

        var json = JsonSerializer.Serialize(request);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        progress?.Report($"Подключение к Ollama ({ModelName})...");

        // ResponseHeadersRead: timeout applies only to receiving headers, NOT to reading the body stream.
        // This prevents TaskCanceledException on long generations.
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{OllamaBaseUrl}/api/generate")
        {
            Content = content
        };

        using var httpResponse = await httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        httpResponse.EnsureSuccessStatusCode();

        // Read NDJSON stream: each line is one OllamaResponse chunk
        await using var bodyStream = await httpResponse.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(bodyStream, Encoding.UTF8);

        var fullText = new StringBuilder();
        int tokenCount = 0;

        progress?.Report($"Генерация документации через {ModelName}...");

        while (!reader.EndOfStream)
        {
            ct.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line)) continue;

            OllamaResponse? chunk;
            try
            {
                chunk = JsonSerializer.Deserialize<OllamaResponse>(line);
            }
            catch (JsonException)
            {
                continue; // skip malformed lines
            }

            if (chunk is null) continue;

            if (!string.IsNullOrEmpty(chunk.Error))
                throw new InvalidOperationException($"Ошибка Ollama: {chunk.Error}");

            fullText.Append(chunk.Response);
            tokenCount++;

            // Report progress every 15 tokens to avoid flooding the UI
            if (tokenCount % 15 == 0)
                progress?.Report($"Генерация... {tokenCount} токенов · {fullText.Length} символов");

            if (chunk.Done) break;
        }

        var result = fullText.ToString().Trim();

        if (string.IsNullOrWhiteSpace(result))
            throw new InvalidOperationException($"Модель {ModelName} вернула пустой ответ. Проверьте, что модель загружена (оллама лист {ModelName}).");

        progress?.Report($"Документация готова — {tokenCount} токенов.");
        return result;
    }

    public static string GetOllamaSetupInstructions() =>
        """
        Ollama недоступна на localhost:11434.
        
        Для запуска Ollama:
        
        1. Установите Ollama с https://ollama.ai/
        
        2. Запустите Ollama (если не запущен в фоне):
           ollama serve
        
        3. Загрузите модель llama3 (если ещё не загружена):
           ollama pull llama3
        
        4. Убедитесь, что Ollama работает:
           curl http://localhost:11434/api/tags
        
        После запуска повторите запуск AudioPM.Local.
        """;
}
