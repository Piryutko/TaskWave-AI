<script lang="ts">
  const steps = [
    {
      icon: "🎙",
      title: "Загрузите аудио",
      desc: "Любой формат: MP3, WAV, M4A, OGG, FLAC. Запись Zoom, Teams, Google Meet — всё подойдёт.",
    },
    {
      icon: "🤖",
      title: "AI обрабатывает",
      desc: "Наш AI расшифровывает речь, исправляет технический жаргон и создаёт структурированный документ.",
    },
    {
      icon: "📋",
      title: "Получите документ",
      desc: "Эпики, задачи, DoD и Acceptance Criteria. Готово к работе в Jira и Linear.",
    },
  ];

  const features = [
    {
      icon: "🔒",
      title: "100% приватно",
      desc: "Все вычисления — на вашем сервере. Данные не покидают инфраструктуру.",
    },
    {
      icon: "⚡",
      title: "Два режима",
      desc: "Иерархия эпиков или плоский список задач — на ваш выбор.",
    },
    {
      icon: "✅",
      title: "DoD и AC",
      desc: "Definition of Done и Acceptance Criteria для каждой задачи опционально.",
    },
    {
      icon: "🔊",
      title: "Любой формат",
      desc: "MP3, M4A, OGG, FLAC, WAV — любой формат автоматически обрабатывается.",
    },
    {
      icon: "🧠",
      title: "Выбор модели",
      desc: "Несколько уровней точности распознавания — под вашу задачу и мощность сервера.",
    },
    {
      icon: "🚀",
      title: "Live прогресс",
      desc: "Наблюдайте за обработкой в реальном времени — токен за токеном.",
    },
  ];
</script>

<svelte:head>
  <title>TaskWave AI — Митинги в задачи автоматически</title>
  <meta
    name="description"
    content="TaskWave AI превращает записи встреч в структурированную проектную документацию с эпиками, задачами и критериями приёмки."
  />
  <link rel="preconnect" href="https://fonts.googleapis.com" />
  <link
    href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap"
    rel="stylesheet"
  />
</svelte:head>

<!-- Hero -->
<section class="wave-bg relative overflow-hidden">
  <div class="max-w-6xl mx-auto px-6 pt-28 pb-24 text-center">
    <!-- Badge -->
    <div
      class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full mb-8 text-sm font-medium
                animate-slide-up"
      style="background: rgba(124,58,237,0.12); border: 1px solid rgba(124,58,237,0.3); color: #c4b5fd;"
    >
      <span class="w-2 h-2 rounded-full bg-violet-400 animate-pulse-glow"
      ></span>
      AI-powered встречи в задачи
    </div>

    <!-- Heading -->
    <h1
      class="text-5xl sm:text-7xl font-extrabold tracking-tight text-white mb-6
               animate-slide-up"
      style="animation-delay:0.1s; line-height:1.1;"
    >
      Встречи —<br />
      <span
        style="background: linear-gradient(135deg,#7c3aed,#22d3ee);
                   -webkit-background-clip:text; -webkit-text-fill-color:transparent;"
      >
        в задачи.
      </span><br />
      Автоматически.
    </h1>

    <p
      class="text-xl text-slate-400 max-w-2xl mx-auto mb-10 animate-slide-up"
      style="animation-delay:0.2s;"
    >
      Загружайте запись встречи — TaskWave расшифрует, исправит технические
      термины и создаст готовый документ с эпиками, задачами и Acceptance
      Criteria.
    </p>

    <div
      class="flex items-center justify-center gap-4 animate-slide-up"
      style="animation-delay:0.3s;"
    >
      <a href="/upload" class="btn-primary text-base px-7 py-3.5">
        🎙 Загрузить аудио
      </a>
    </div>

    <!-- Preview card -->
    <div
      class="mt-20 max-w-3xl mx-auto animate-slide-up"
      style="animation-delay:0.4s;"
    >
      <div class="card glow-purple relative overflow-hidden">
        <div
          class="absolute inset-0"
          style="background: linear-gradient(135deg, rgba(124,58,237,0.05) 0%, transparent 60%);"
        ></div>
        <div class="relative">
          <div class="flex items-center gap-2 mb-4">
            <div class="w-3 h-3 rounded-full bg-red-500/60"></div>
            <div class="w-3 h-3 rounded-full bg-yellow-500/60"></div>
            <div class="w-3 h-3 rounded-full bg-green-500/60"></div>
            <span class="ml-3 text-xs text-slate-500 font-mono"
              >AudioPM_standup_20260306.txt</span
            >
          </div>
          <div class="text-left font-mono text-sm space-y-1 text-slate-300">
            <div class="text-violet-400 font-bold">
              ═══ ЭПИК 1: Рефакторинг API ═══
            </div>
            <div class="text-slate-400">
              📋 Задача 1.1 — Переписать эндпоинт /analyze
            </div>
            <div class="text-slate-500 text-xs ml-8">
              DoD: unit-тесты покрывают 80%+ кода
            </div>
            <div class="text-slate-500 text-xs ml-8">
              AC: &nbsp;ответ за &lt;200ms при нагрузке 100 rps
            </div>
            <div class="text-slate-400">
              📋 Задача 1.2 — Добавить SSE стриминг прогресса
            </div>
            <div class="text-slate-500 text-xs ml-8">
              DoD: интеграционные тесты пройдены
            </div>
            <div class="mt-3 text-cyan-400 font-bold">
              ═══ ЭПИК 2: Фронтенд SvelteKit ═══
            </div>
            <div class="text-slate-400">
              📋 Задача 2.1 — Landing page и Upload экран
            </div>
            <div class="text-slate-400">
              📋 Задача 2.2 — Live progress bar через SSE
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</section>

<!-- How it works -->
<section class="max-w-6xl mx-auto px-6 py-24">
  <div class="text-center mb-16">
    <h2 class="text-4xl font-bold text-white mb-4">Как это работает</h2>
    <p class="text-slate-400 text-lg">Три шага — и документ готов</p>
  </div>

  <div class="grid md:grid-cols-3 gap-6">
    {#each steps as step, i}
      <div class="card card-hover relative">
        <div class="step-dot mb-5">{i + 1}</div>
        <div class="text-3xl mb-3">{step.icon}</div>
        <h3 class="text-xl font-semibold text-white mb-2">{step.title}</h3>
        <p class="text-slate-400 leading-relaxed">{step.desc}</p>
      </div>
    {/each}
  </div>
</section>

<!-- Features -->
<section class="max-w-6xl mx-auto px-6 py-24 border-t border-white/5">
  <div class="text-center mb-16">
    <h2 class="text-4xl font-bold text-white mb-4">Возможности</h2>
  </div>

  <div class="grid sm:grid-cols-2 lg:grid-cols-3 gap-5">
    {#each features as f}
      <div class="card card-hover flex gap-4">
        <div class="text-2xl flex-shrink-0">{f.icon}</div>
        <div>
          <h3 class="font-semibold text-white mb-1">{f.title}</h3>
          <p class="text-sm text-slate-400 leading-relaxed">{f.desc}</p>
        </div>
      </div>
    {/each}
  </div>
</section>

<!-- CTA -->
<section class="max-w-6xl mx-auto px-6 py-24">
  <div class="card glow-purple text-center relative overflow-hidden">
    <div
      class="absolute inset-0"
      style="background: linear-gradient(135deg, rgba(124,58,237,0.1) 0%, rgba(34,211,238,0.05) 100%);"
    ></div>
    <div class="relative">
      <h2 class="text-4xl font-bold text-white mb-4">Готовы начать?</h2>
      <p class="text-slate-400 text-lg mb-8 max-w-xl mx-auto">
        Загрузите любую запись встречи и получите готовый документ за минуты.
        MP3, WAV, M4A, OGG — любой формат.
      </p>
      <a href="/upload" class="btn-primary text-base px-8 py-3.5">
        🎙 Загрузить аудио бесплатно
      </a>
    </div>
  </div>
</section>
