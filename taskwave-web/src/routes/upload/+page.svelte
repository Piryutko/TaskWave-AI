<script lang="ts">
  import { goto } from "$app/navigation";
  import { API_BASE } from "$lib/config";

  let dragOver = false;
  let selectedFile: File | null = null;
  let mode: "epics" | "tasks" = "epics";
  let includeDodAc = true;
  let uploading = false;
  let error = "";

  function handleDrop(e: DragEvent) {
    e.preventDefault();
    dragOver = false;
    const file = e.dataTransfer?.files[0];
    if (file) selectFile(file);
  }

  function handleFileInput(e: Event) {
    const file = (e.target as HTMLInputElement).files?.[0];
    if (file) selectFile(file);
  }

  function selectFile(file: File) {
    const allowed = [
      "audio/mpeg",
      "audio/wav",
      "audio/ogg",
      "audio/flac",
      "audio/mp4",
      "audio/x-m4a",
      "audio/aac",
      "video/mp4",
    ];
    if (
      !allowed.includes(file.type) &&
      !file.name.match(/\.(mp3|wav|ogg|flac|m4a|aac)$/i)
    ) {
      error =
        "Неподдерживаемый формат. Используйте MP3, WAV, M4A, OGG или FLAC.";
      return;
    }
    error = "";
    selectedFile = file;
  }

  async function submit() {
    if (!selectedFile) return;
    uploading = true;
    error = "";
    try {
      const form = new FormData();
      form.append("audio", selectedFile);
      form.append("mode", mode);
      form.append("dodAc", String(includeDodAc));

      const res = await fetch(`${API_BASE}/api/jobs`, {
        method: "POST",
        body: form,
      });

      if (!res.ok) {
        const err = await res.text();
        throw new Error(err || `HTTP ${res.status}`);
      }

      const { jobId } = await res.json();
      goto(`/processing/${jobId}`);
    } catch (e: any) {
      error =
        e.message ??
        "Ошибка при отправке. Убедитесь что API запущен на порту 5000.";
      uploading = false;
    }
  }

  function formatBytes(b: number) {
    if (b < 1024) return b + " B";
    if (b < 1024 * 1024) return (b / 1024).toFixed(1) + " KB";
    return (b / 1024 / 1024).toFixed(1) + " MB";
  }
</script>

<svelte:head>
  <title>Загрузить аудио — TaskWave AI</title>
</svelte:head>

<div class="max-w-2xl mx-auto px-6 py-16">
  <div class="mb-10">
    <div class="badge mb-4">Шаг 1 из 2</div>
    <h1 class="text-4xl font-bold text-white mb-3">Загрузите аудио</h1>
    <p class="text-slate-400 text-lg">
      Запись встречи в любом формате — мы разберёмся с остальным
    </p>
  </div>

  <!-- Drop zone -->
  <!-- svelte-ignore a11y-no-static-element-interactions -->
  <div
    class="card mb-6 cursor-pointer transition-all duration-200 relative overflow-hidden
              {dragOver
      ? 'border-violet-500 glow-purple'
      : 'border-white/10 hover:border-white/20'}"
    on:dragover|preventDefault={() => (dragOver = true)}
    on:dragleave={() => (dragOver = false)}
    on:drop={handleDrop}
    on:click={() => document.getElementById("fileInput")?.click()}
  >
    {#if selectedFile}
      <div class="flex items-center gap-4">
        <div
          class="w-14 h-14 rounded-2xl flex items-center justify-center text-2xl flex-shrink-0"
          style="background: rgba(124,58,237,0.2);"
        >
          🎙
        </div>
        <div class="flex-1 min-w-0">
          <p class="font-semibold text-white truncate">{selectedFile.name}</p>
          <p class="text-sm text-slate-400">{formatBytes(selectedFile.size)}</p>
        </div>
        <button
          class="text-slate-500 hover:text-red-400 transition-colors p-2"
          on:click|stopPropagation={() => (selectedFile = null)}>✕</button
        >
      </div>
    {:else}
      <div class="text-center py-8">
        <div class="text-5xl mb-4 {dragOver ? 'animate-float' : ''}">📁</div>
        <p class="text-white font-semibold mb-1">
          {dragOver ? "Отпускайте!" : "Перетащите файл или нажмите"}
        </p>
        <p class="text-sm text-slate-500">
          MP3, WAV, M4A, OGG, FLAC — до 500 МБ
        </p>
      </div>
    {/if}

    <input
      id="fileInput"
      type="file"
      class="hidden"
      accept=".mp3,.wav,.ogg,.flac,.m4a,.aac"
      on:change={handleFileInput}
    />
  </div>

  {#if error}
    <div
      class="mb-6 p-4 rounded-xl text-red-400 text-sm"
      style="background: rgba(239,68,68,0.1); border: 1px solid rgba(239,68,68,0.3);"
    >
      ⚠️ {error}
    </div>
  {/if}

  <!-- Mode selection -->
  <div class="card mb-6">
    <h3 class="font-semibold text-white mb-4">Режим анализа</h3>
    <div class="grid grid-cols-2 gap-3">
      {#each [{ id: "epics", label: "Эпики и декомпозиция", icon: "🗂", desc: "Иерархия: Эпик → Задача → Подзадача" }, { id: "tasks", label: "Список задач", icon: "📋", desc: "Плоский нумерованный список" }] as m}
        <button
          class="p-4 rounded-xl text-left transition-all duration-200 border
                       {mode === m.id
            ? 'border-violet-500 bg-violet-500/10 text-white'
            : 'border-white/10 text-slate-400 hover:border-white/20 hover:text-white'}"
          on:click={() => (mode = m.id as "epics" | "tasks")}
        >
          <div class="text-xl mb-2">{m.icon}</div>
          <div class="font-medium text-sm mb-1">{m.label}</div>
          <div class="text-xs opacity-70">{m.desc}</div>
        </button>
      {/each}
    </div>
  </div>

  <!-- Options -->
  <div class="card mb-8">
    <h3 class="font-semibold text-white mb-4">Опции</h3>
    <label class="flex items-center gap-3 cursor-pointer">
      <div class="relative">
        <input type="checkbox" class="sr-only" bind:checked={includeDodAc} />
        <div
          class="w-11 h-6 rounded-full transition-colors duration-200
                    {includeDodAc ? 'bg-violet-600' : 'bg-white/10'}"
        ></div>
        <div
          class="absolute top-0.5 left-0.5 w-5 h-5 rounded-full bg-white shadow transition-transform duration-200
                    {includeDodAc ? 'translate-x-5' : ''}"
        ></div>
      </div>
      <div>
        <div class="text-white font-medium text-sm">
          Definition of Done и Acceptance Criteria
        </div>
        <div class="text-slate-500 text-xs">
          Добавить DoD и AC для каждой задачи
        </div>
      </div>
    </label>
  </div>

  <!-- Submit -->
  <button
    class="btn-primary w-full justify-center py-4 text-base
                 {!selectedFile || uploading
      ? 'opacity-50 cursor-not-allowed'
      : ''}"
    disabled={!selectedFile || uploading}
    on:click={submit}
  >
    {#if uploading}
      <div
        class="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin-slow"
      ></div>
      Отправка...
    {:else}
      🚀 Начать анализ
    {/if}
  </button>
</div>
