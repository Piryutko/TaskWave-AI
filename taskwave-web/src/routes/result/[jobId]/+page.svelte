<script lang="ts">
  import { page } from "$app/stores";
  import { onMount } from "svelte";

  const jobId = $page.params.jobId;
  const isDemo = jobId === "demo-job";

  let resultText = "";
  let loading = true;
  let error = "";
  let epicCount = 0;
  let taskCount = 0;
  let dodCount = 0;
  let copied = false;

  const demoResult = `═══ ЭПИК 1: Рефакторинг API ═══

📋 Задача 1.1 — Переписать эндпоинт /analyze
   DoD: Unit-тесты покрывают 80%+ кода. Code review пройден.
   AC:  Ответ за <200ms при нагрузке 100 rps.

📋 Задача 1.2 — Добавить SSE стриминг прогресса
   DoD: Интеграционные тесты пройдены.
   AC:  Токены поступают без задержки >50ms.

═══ ЭПИК 2: Фронтенд SvelteKit ═══

📋 Задача 2.1 — Landing page и Upload экран
📋 Задача 2.2 — Live progress bar через SSE`;

  function analyzeText(text: string) {
    epicCount = (text.match(/═══|ЭПИК\s+\d+/gi) ?? []).length;
    taskCount = (text.match(/📋|Задача\s+\d+/gi) ?? []).length;
    dodCount = (text.match(/\b(DoD|AC|Definition of Done|Критерии)/gi) ?? [])
      .length;
    if (epicCount === 0 && taskCount === 0) {
      // Fallback: count lines with dashes or numbers
      taskCount = (text.match(/^\s*[-\d]/gm) ?? []).length;
    }
  }

  onMount(async () => {
    if (isDemo) {
      resultText = demoResult;
      analyzeText(resultText);
      loading = false;
      return;
    }

    // Fetch real result from API
    try {
      const res = await fetch(`http://localhost:5000/api/jobs/${jobId}/result`);
      if (res.status === 202) {
        error = "Документ ещё генерируется. Подождите...";
        loading = false;
        return;
      }
      if (!res.ok) {
        const err = await res
          .json()
          .catch(() => ({ error: `HTTP ${res.status}` }));
        throw new Error(err.error ?? `HTTP ${res.status}`);
      }
      const data = await res.json();
      resultText = data.result ?? "";
      analyzeText(resultText);
    } catch (e: any) {
      error = e.message ?? "Не удалось загрузить результат.";
    } finally {
      loading = false;
    }
  });

  async function copyText() {
    await navigator.clipboard.writeText(resultText);
    copied = true;
    setTimeout(() => (copied = false), 2000);
  }

  function downloadTxt() {
    const blob = new Blob([resultText], { type: "text/plain;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `TaskWave_${new Date().toISOString().slice(0, 10)}.txt`;
    a.click();
    URL.revokeObjectURL(url);
  }
</script>

<svelte:head>
  <title>Результат — TaskWave AI</title>
</svelte:head>

<div class="max-w-4xl mx-auto px-6 py-16">
  {#if loading}
    <!-- Loading state -->
    <div class="flex flex-col items-center justify-center py-24 gap-4">
      <div
        class="w-10 h-10 border-2 border-violet-400/30 border-t-violet-400 rounded-full animate-spin-slow"
      ></div>
      <p class="text-slate-400">Загрузка результата...</p>
    </div>
  {:else if error}
    <!-- Error state -->
    <div
      class="card"
      style="border-color: rgba(239,68,68,0.4); background: rgba(239,68,68,0.05);"
    >
      <p class="text-red-400 font-medium mb-2">⚠️ {error}</p>
      <a href="/upload" class="btn-ghost text-sm py-2 mt-3 inline-flex"
        >← Загрузить снова</a
      >
    </div>
  {:else}
    <!-- Header -->
    <div class="flex items-start justify-between gap-4 mb-8 flex-wrap">
      <div>
        <div
          class="badge mb-4"
          style="background: rgba(34,197,94,0.12); border-color: rgba(34,197,94,0.3); color: #4ade80;"
        >
          ✓ Документ готов
        </div>
        <h1 class="text-4xl font-bold text-white mb-2">
          Проектная документация
        </h1>
        <p class="text-slate-400">
          {isDemo ? "Демо-результат" : `Job: ${jobId}`}
          · {new Date().toLocaleDateString("ru-RU", {
            day: "numeric",
            month: "long",
            year: "numeric",
          })}
        </p>
      </div>

      <div class="flex items-center gap-3 flex-shrink-0">
        <button class="btn-ghost text-sm py-2.5 px-4" on:click={copyText}>
          {copied ? "✓ Скопировано!" : "📋 Копировать"}
        </button>
        <button class="btn-primary text-sm py-2.5 px-4" on:click={downloadTxt}>
          ⬇️ Скачать .txt
        </button>
      </div>
    </div>

    <!-- Stats -->
    {#if epicCount > 0 || taskCount > 0}
      <div class="grid grid-cols-3 gap-4 mb-8">
        {#each [{ label: "Эпиков", value: epicCount, icon: "🗂" }, { label: "Задач", value: taskCount, icon: "📋" }, { label: "DoD/AC", value: dodCount, icon: "✅" }] as stat}
          <div class="card text-center">
            <div class="text-2xl mb-1">{stat.icon}</div>
            <div class="text-3xl font-bold text-white">{stat.value}</div>
            <div class="text-sm text-slate-500 mt-1">{stat.label}</div>
          </div>
        {/each}
      </div>
    {/if}

    <!-- Document -->
    <div class="card">
      <pre
        class="font-mono text-sm text-slate-300 whitespace-pre-wrap leading-relaxed overflow-x-auto"
        style="font-family: 'JetBrains Mono', 'Fira Code', 'Courier New', monospace;">{resultText}</pre>
    </div>

    <!-- Actions -->
    <div class="mt-8 flex items-center justify-between flex-wrap gap-4">
      <a href="/upload" class="btn-ghost text-sm py-2.5">← Обработать ещё</a>
      <div class="text-sm text-slate-500">
        🔒 Документ создан локально — никаких данных в облаке
      </div>
    </div>
  {/if}
</div>
