using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using TaskWave.Api.Models;
using TaskWave.Api.Services;

namespace TaskWave.Api.Jobs;

/// <summary>
/// Manages job lifecycle: creation, background processing, SSE event dispatch.
/// </summary>
public sealed class JobManager(WhisperService whisper, OllamaService ollama, ILogger<JobManager> logger)
{
    private readonly ConcurrentDictionary<string, JobState> _jobs = new();

    public JobState CreateJob(string fileName, string mode, bool includeDodAc, string tempAudioPath)
    {
        var job = new JobState
        {
            FileName      = fileName,
            Mode          = mode,
            IncludeDodAc  = includeDodAc,
            TempAudioPath = tempAudioPath,
        };
        _jobs[job.Id] = job;
        return job;
    }

    public JobState? GetJob(string id) =>
        _jobs.TryGetValue(id, out var job) ? job : null;

    /// <summary>Starts background processing. Fires and forgets — progress via SSE channel.</summary>
    public void StartProcessing(JobState job, CancellationToken appStopping = default)
    {
        _ = Task.Run(() => ProcessAsync(job, appStopping), appStopping);
    }

    private async Task ProcessAsync(JobState job, CancellationToken ct)
    {
        job.Status = JobStatus.Processing;

        try
        {
            // ── Step 1: Whisper init ─────────────────────────────────────────
            await RunStep(job, StepId.Whisper, async progress =>
            {
                await whisper.InitializeAsync(progress);
            }, ct);

            // ── Step 2: Transcription ────────────────────────────────────────
            string rawText = string.Empty;
            await RunStep(job, StepId.Transcribe, async progress =>
            {
                rawText = await whisper.TranscribeAsync(job.TempAudioPath, progress, ct);
            }, ct);

            // ── Step 3: AI Corrector ─────────────────────────────────────────
            string cleanText = rawText;
            await RunStep(job, StepId.Correct, async progress =>
            {
                cleanText = await ollama.RefineTranscriptionAsync(rawText, progress, ct);
                // Also push token events so frontend can animate
            }, ct);

            // ── Step 4: Generate docs ────────────────────────────────────────
            await RunStep(job, StepId.Generate, async progress =>
            {
                job.ResultText = await ollama.GenerateDocumentationAsync(
                    cleanText, job.Mode, job.IncludeDodAc, progress, ct);
            }, ct);

            job.Status = JobStatus.Done;
            PushEvent(job, new { type = "done" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Job {JobId} failed", job.Id);
            job.Status       = JobStatus.Failed;
            job.ErrorMessage = ex.Message;
            PushEvent(job, new { type = "error", message = ex.Message });
        }
        finally
        {
            // Clean up temp audio file
            if (File.Exists(job.TempAudioPath))
                try { File.Delete(job.TempAudioPath); } catch { /* best-effort */ }

            job.Complete();
        }
    }

    private async Task RunStep(JobState job, StepId stepId, Func<IProgress<string>, Task> work, CancellationToken ct)
    {
        var step = job.Steps.First(s => s.Id == stepId);
        step.Status = "running";
        PushStepEvent(job, step);

        var progress = new Progress<string>(detail =>
        {
            step.Detail = detail;
            PushStepEvent(job, step);
        });

        await work(progress);

        step.Status   = "done";
        step.Progress = 100;
        PushStepEvent(job, step);
    }

    private void PushStepEvent(JobState job, StepState step) =>
        PushEvent(job, new
        {
            type     = "step",
            step     = step.Id.ToString().ToLower(),
            status   = step.Status,
            progress = step.Progress,
            detail   = step.Detail,
        });

    private void PushEvent(JobState job, object payload) =>
        job.PushEvent(JsonSerializer.Serialize(payload));
}
