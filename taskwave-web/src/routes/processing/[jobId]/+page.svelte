<script lang="ts">
  import { onMount, onDestroy } from "svelte";
  import { page } from "$app/stores";
  import { goto } from "$app/navigation";

  const jobId = $page.params.jobId;
  const isDemo = jobId === "demo-job";

  type StepStatus = "pending" | "running" | "done" | "error";

  interface Step {
    id: string;
    label: string;
    sublabel: string;
    status: StepStatus;
    detail: string;
    progress: number; // 0-100
  }

  let steps: Step[] = [
    {
      id: "whisper",
      label: "Инициализация AI-модели",
      sublabel: "Загрузка модели распознавания...",
      status: "pending",
      detail: "",
      progress: 0,
    },
    {
      id: "transcribe",
      label: "Расшифровка аудио",
      sublabel: "Локальная обработка речи...",
      status: "pending",
      detail: "",
      progress: 0,
    },
    {
      id: "correct",
      label: "ИИ исправляет ошибки",
      sublabel: "Коррекция технических терминов...",
      status: "pending",
      detail: "",
      progress: 0,
    },
    {
      id: "generate",
      label: "Генерация документации",
      sublabel: "LLM аналитик формирует задачи...",
      status: "pending",
      detail: "",
      progress: 0,
    },
  ];

  let currentStep = 0;
  let globalProgress = 0;
  let finished = false;
  let hasError = false;
  let tokenCount = 0;
  let elapsedSeconds = 0;
  let interval: ReturnType<typeof setInterval>;
  let evtSource: EventSource | null = null;

  function updateProgress() {
    globalProgress =
      steps.reduce((sum, s) => sum + s.progress, 0) / steps.length;
  }

  // ── Demo simulation ─────────────────────────────────────────────────────────
  async function runDemo() {
    for (let i = 0; i < steps.length; i++) {
      currentStep = i;
      steps[i].status = "running";
      steps = steps; // trigger reactivity

      // Simulate progress fill
      for (let p = 0; p <= 100; p += 5) {
        await delay(i === 3 ? 120 : 60);
        steps[i].progress = p;
        if (i === 3) {
          tokenCount += 7;
          steps[i].detail = `${tokenCount} токенов`;
        }
        updateProgress();
        steps = steps;
      }
      steps[i].status = "done";
      steps = steps;
    }
    finished = true;
    await delay(800);
    goto("/result/demo-job");
  }

  // ── Real SSE ─────────────────────────────────────────────────────────────────
  function connectSSE() {
    evtSource = new EventSource(
      `http://localhost:5000/api/jobs/${jobId}/stream`,
    );

    evtSource.onmessage = (e) => {
      try {
        const data = JSON.parse(e.data);

        if (data.type === "step") {
          const idx = steps.findIndex((s) => s.id === data.step);
          if (idx !== -1) {
            steps[idx].status = data.status;
            steps[idx].detail = data.detail ?? "";
            steps[idx].progress =
              data.progress ?? (data.status === "done" ? 100 : 0);
            currentStep = idx;
            updateProgress();
            steps = steps;
          }
        }

        if (data.type === "done") {
          finished = true;
          evtSource?.close();
          setTimeout(() => goto(`/result/${jobId}`), 800);
        }

        if (data.type === "error") {
          hasError = true;
          evtSource?.close();
        }
      } catch {
        /* ignore parse errors */
      }
    };

    evtSource.onerror = () => {
      hasError = true;
      evtSource?.close();
    };
  }

  const delay = (ms: number) => new Promise((r) => setTimeout(r, ms));

  onMount(() => {
    interval = setInterval(() => elapsedSeconds++, 1000);
    if (isDemo) runDemo();
    else connectSSE();
  });

  onDestroy(() => {
    clearInterval(interval);
    evtSource?.close();
  });

  function formatTime(s: number) {
    const m = Math.floor(s / 60);
    const sec = s % 60;
    return m > 0 ? `${m}м ${sec}с` : `${sec}с`;
  }

  const statusIcon: Record<StepStatus, string> = {
    pending: "○",
    running: "◌",
    done: "✓",
    error: "✕",
  };
</script>

<svelte:head>
  <title>Обработка... — TaskWave AI</title>
</svelte:head>

<div class="max-w-2xl mx-auto px-6 py-16">
  <div class="mb-10">
    <div class="badge mb-4">
      {#if finished}✓ Завершено{:else}Обработка...{/if}
    </div>
    <h1 class="text-3xl font-bold text-white mb-2">
      {finished ? "Документ готов!" : "TaskWave анализирует встречу"}
    </h1>
    <p class="text-slate-400">
      Прошло: {formatTime(elapsedSeconds)}
      {#if globalProgress > 0}
        · {Math.round(globalProgress)}% завершено
      {/if}
    </p>
  </div>

  <!-- Global progress bar -->
  <div
    class="mb-8 h-1.5 rounded-full overflow-hidden"
    style="background: rgba(255,255,255,0.07);"
  >
    <div
      class="progress-bar-fill h-full"
      style="width: {globalProgress}%;"
    ></div>
  </div>

  <!-- Steps -->
  <div class="space-y-4 mb-10">
    {#each steps as step, i}
      <div
        class="card transition-all duration-300
                  {step.status === 'running'
          ? 'border-violet-500/50 glow-purple'
          : ''}
                  {step.status === 'done' ? 'border-green-500/30' : ''}
                  {step.status === 'pending' ? 'opacity-50' : ''}"
      >
        <div class="flex items-start gap-4">
          <!-- Status icon -->
          <div
            class="w-10 h-10 rounded-xl flex items-center justify-center text-lg flex-shrink-0 font-bold
                      {step.status === 'done'
              ? 'bg-green-500/20 text-green-400'
              : ''}
                      {step.status === 'running'
              ? 'bg-violet-500/20 text-violet-400'
              : ''}
                      {step.status === 'pending'
              ? 'bg-white/5      text-slate-600'
              : ''}
                      {step.status === 'error'
              ? 'bg-red-500/20   text-red-400'
              : ''}"
          >
            {#if step.status === "running"}
              <div
                class="w-5 h-5 border-2 border-violet-400/40 border-t-violet-400 rounded-full animate-spin-slow"
              ></div>
            {:else}
              {statusIcon[step.status]}
            {/if}
          </div>

          <div class="flex-1 min-w-0">
            <div class="flex items-center justify-between gap-2 mb-1">
              <span class="font-semibold text-white">{step.label}</span>
              {#if step.detail}
                <span class="text-xs text-slate-500 flex-shrink-0"
                  >{step.detail}</span
                >
              {/if}
            </div>
            <p class="text-sm text-slate-400">{step.sublabel}</p>

            {#if step.status === "running" && step.progress > 0}
              <div
                class="mt-3 h-1 rounded-full overflow-hidden"
                style="background: rgba(255,255,255,0.07);"
              >
                <div
                  class="progress-bar-fill h-full"
                  style="width: {step.progress}%;"
                ></div>
              </div>
            {/if}
          </div>
        </div>
      </div>
    {/each}
  </div>

  {#if hasError}
    <div
      class="card"
      style="border-color: rgba(239,68,68,0.4); background: rgba(239,68,68,0.05);"
    >
      <p class="text-red-400 font-medium mb-2">⚠️ Произошла ошибка</p>
      <p class="text-sm text-slate-400 mb-4">
        Проверьте что AI-сервис запущен и модель загружена.
      </p>
      <a href="/upload" class="btn-ghost text-sm py-2">← Попробовать снова</a>
    </div>
  {:else if !finished}
    <p class="text-center text-sm text-slate-500">
      🔒 Всё обрабатывается локально — данные не покидают ваш сервер
    </p>
  {/if}
</div>
