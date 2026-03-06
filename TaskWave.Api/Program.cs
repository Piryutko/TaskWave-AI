using Microsoft.AspNetCore.Http.Features;
using TaskWave.Api.Jobs;
using TaskWave.Api.Models;
using TaskWave.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────

builder.Services.AddSingleton<WhisperService>();
builder.Services.AddSingleton<JobManager>();

// OllamaService as singleton with its own long-timeout HttpClient
builder.Services.AddSingleton<OllamaService>(_ =>
    new OllamaService(new HttpClient { Timeout = TimeSpan.FromMinutes(10) }));

// Auto-select Whisper model at startup
builder.Services.AddHostedService<WhisperModelStartup>();

// Increase max upload size for audio files (up to 500 MB)
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 500 * 1024 * 1024; // 500 MB
});

// CORS — allow SvelteKit dev server + production
builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(p =>
        p.WithOrigins("http://localhost:5173", "https://taskwave.io")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials()));

var app = builder.Build();

app.UseCors();

// ── Endpoints ─────────────────────────────────────────────────────────────────

// Health / Ollama status
app.MapGet("/api/health", async (OllamaService ollama) => new
{
    ok    = true,
    ollama = await ollama.IsAvailableAsync(),
    models = await ollama.GetAvailableModelsAsync(),
});

// POST /api/jobs — upload audio + settings, returns jobId
app.MapPost("/api/jobs", async (
    HttpRequest request,
    JobManager jobs,
    OllamaService ollama) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest("Expected multipart/form-data");

    var form       = await request.ReadFormAsync();
    var audioFile  = form.Files["audio"];
    var mode       = form["mode"].ToString() is { Length: > 0 } m ? m : "epics";
    var includeDodAc = form["dodAc"].ToString() != "false";
    var modelName  = form["model"].ToString() is { Length: > 0 } mn ? mn : ollama.ModelName;

    if (audioFile is null || audioFile.Length == 0)
        return Results.BadRequest("No audio file provided");

    ollama.ModelName = modelName;

    // Save upload to temp file
    var tempPath = Path.Combine(Path.GetTempPath(), $"tw_{Guid.NewGuid():N}{Path.GetExtension(audioFile.FileName)}");
    await using (var fs = File.Create(tempPath))
        await audioFile.CopyToAsync(fs);

    var job = jobs.CreateJob(audioFile.FileName, mode, includeDodAc, tempPath);
    jobs.StartProcessing(job, app.Lifetime.ApplicationStopping);

    return Results.Ok(new { jobId = job.Id });
});

// GET /api/jobs/{id}/stream — SSE progress stream
app.MapGet("/api/jobs/{id}/stream", async (string id, JobManager jobs, HttpContext ctx) =>
{
    var job = jobs.GetJob(id);
    if (job is null) return Results.NotFound();

    ctx.Response.Headers.ContentType  = "text/event-stream";
    ctx.Response.Headers.CacheControl = "no-cache";
    ctx.Response.Headers["X-Accel-Buffering"] = "no"; // disable nginx buffering

    await ctx.Response.Body.FlushAsync();

    var ct = ctx.RequestAborted;

    try
    {
        await foreach (var eventJson in job.EventReader.ReadAllAsync(ct))
        {
            await ctx.Response.WriteAsync($"data: {eventJson}\n\n", ct);
            await ctx.Response.Body.FlushAsync(ct);
        }
    }
    catch (OperationCanceledException) { /* client disconnected */ }

    return Results.Empty;
});

// GET /api/jobs/{id}/result — fetch final document
app.MapGet("/api/jobs/{id}/result", (string id, JobManager jobs) =>
{
    var job = jobs.GetJob(id);
    if (job is null) return Results.NotFound();
    if (job.Status == JobStatus.Failed)  return Results.BadRequest(new { error = job.ErrorMessage });
    if (job.Status != JobStatus.Done)    return Results.StatusCode(202);

    return Results.Ok(new
    {
        jobId  = job.Id,
        result = job.ResultText,
        file   = job.FileName,
        mode   = job.Mode,
        dodAc  = job.IncludeDodAc,
    });
});

// GET /api/models — list Ollama models
app.MapGet("/api/models", async (OllamaService ollama) =>
    Results.Ok(await ollama.GetAvailableModelsAsync()));

app.Run();

// ── Hosted service: auto-select Whisper model on startup ─────────────────────

public sealed class WhisperModelStartup(
    WhisperService whisper,
    OllamaService ollama,
    ILogger<WhisperModelStartup> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        // Auto-select Whisper model
        var found = whisper.FindAnyPresentModel();
        if (found is not null)
        {
            _ = whisper.SetModelAsync(found);
            logger.LogInformation("Whisper model auto-selected: {Model}", found.DisplayName);
        }
        else
        {
            logger.LogWarning("No Whisper model found. Download one via the AudioPM.Local CLI.");
        }

        // Auto-select first available Ollama model
        var models = await ollama.GetAvailableModelsAsync(ct);
        if (models.Count > 0)
        {
            ollama.ModelName = models[0];
            logger.LogInformation("Ollama model auto-selected: {Model}", ollama.ModelName);
        }
        else
        {
            logger.LogWarning("No Ollama models found. Run: ollama pull llama3.2");
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
