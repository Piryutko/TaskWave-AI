namespace TaskWave.Api.Jobs;

public enum JobStatus { Queued, Processing, Done, Failed }

public enum StepId { Whisper, Transcribe, Correct, Generate }

public record StepState(StepId Id, string Label, string SubLabel)
{
    public string Status   { get; set; } = "pending"; // pending | running | done | error
    public int    Progress { get; set; } = 0;
    public string Detail   { get; set; } = string.Empty;
}

public class JobState
{
    public string    Id            { get; init; } = Guid.NewGuid().ToString("N")[..12];
    public JobStatus Status        { get; set; } = JobStatus.Queued;
    public string    FileName      { get; init; } = string.Empty;
    public string    Mode          { get; init; } = "epics";
    public bool      IncludeDodAc  { get; init; } = true;
    public string    TempAudioPath { get; set; } = string.Empty;
    public string    ResultText    { get; set; } = string.Empty;
    public string    ErrorMessage  { get; set; } = string.Empty;
    public DateTime  CreatedAt     { get; init; } = DateTime.UtcNow;

    public List<StepState> Steps { get; init; } =
    [
        new(StepId.Whisper,    "Инициализация AI-модели",       "Загрузка модели распознавания..."),
        new(StepId.Transcribe, "Расшифровка аудио",             "Локальная обработка речи..."),
        new(StepId.Correct,    "ИИ исправляет ошибки",          "Коррекция технических терминов..."),
        new(StepId.Generate,   "Генерация документации",         "AI аналитик формирует задачи..."),
    ];

    // SSE subscribers — each job has a channel of string events
    private readonly System.Threading.Channels.Channel<string> _channel =
        System.Threading.Channels.Channel.CreateUnbounded<string>();

    public System.Threading.Channels.ChannelWriter<string>  EventWriter => _channel.Writer;
    public System.Threading.Channels.ChannelReader<string>  EventReader => _channel.Reader;

    public void PushEvent(string json) => _channel.Writer.TryWrite(json);

    public void Complete() => _channel.Writer.Complete();
}
