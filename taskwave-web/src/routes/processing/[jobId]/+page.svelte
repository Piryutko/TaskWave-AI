<script lang="ts">
  import { onMount, onDestroy } from "svelte";
  import { page } from "$app/stores";
  import { goto } from "$app/navigation";
  import { t } from "$lib/i18n";

  const jobId = $page.params.jobId;
  const isDemo = true; 

  type StepStatus = "pending" | "running" | "done" | "error";

  interface Step {
    id: string;
    status: StepStatus;
    detail: string;
    progress: number;
  }

  let steps: Step[] = [
    { id: "whisper", status: "pending", detail: "", progress: 0 },
    { id: "transcribe", status: "pending", detail: "", progress: 0 },
    { id: "correct", status: "pending", detail: "", progress: 0 },
    { id: "generate", status: "pending", detail: "", progress: 0 },
  ];

  let currentStep = 0;
  let globalProgress = 0;
  let finished = false;
  let elapsedSeconds = 0;
  let interval: ReturnType<typeof setInterval>;

  function updateProgress() {
    globalProgress = steps.reduce((sum, s) => sum + s.progress, 0) / steps.length;
  }

  async function runDemo() {
    for (let i = 0; i < steps.length; i++) {
      currentStep = i;
      steps[i].status = "running";
      steps = steps;

      let p = 0;
      while (p < 100) {
        const jump = Math.floor(Math.random() * 15) + 5;
        p = Math.min(100, p + jump);
        
        const baseDelay = i === 3 ? 180 : 120;
        const jitter = Math.random() * 150;
        await delay(baseDelay + jitter);
        
        steps[i].progress = p;
        updateProgress();
        steps = steps;
      }
      steps[i].status = "done";
      if (i === 3) steps[i].detail = $t('processing.success');
      steps = steps;
      await delay(500);
    }
    finished = true;
    await delay(1200);
    const params = $page.url.searchParams.toString();
    goto(`/result/job-1403?${params}`);
  }

  const delay = (ms: number) => new Promise((r) => setTimeout(r, ms));

  onMount(() => {
    interval = setInterval(() => elapsedSeconds++, 1000);
    runDemo();
  });

  onDestroy(() => {
    clearInterval(interval);
  });

  function formatTime(s: number) {
    const m = Math.floor(s / 60);
    const sec = s % 60;
    const mLabel = $t('common.m') || 'm';
    const sLabel = $t('common.s') || 's';
    return m > 0 ? `${m}${mLabel} ${sec}${sLabel}` : `${sec}${sLabel}`;
  }
</script>

<svelte:head>
  <title>{$t('processing.title')} — TaskWave AI</title>
</svelte:head>

<div class="h-screen w-full flex flex-col items-center justify-center px-6 overflow-hidden relative">
  
  <div class="absolute inset-0 pointer-events-none z-0">
    <div class="absolute top-[-10%] right-[-10%] w-[40%] h-[40%] bg-accent/5 rounded-full blur-[120px]"></div>
    <div class="absolute bottom-[-10%] left-[-10%] w-[30%] h-[30%] bg-accent/5 rounded-full blur-[100px]"></div>
  </div>

  <div class="w-full max-w-6xl relative z-10">
    <div class="mb-12 flex flex-col items-center text-center">
      <h1 class="text-5xl md:text-6xl font-bold mb-8 tracking-tighter" style="font-family: 'Geist', sans-serif;">
        <span class="text-white/90">TaskWave</span> 
        <span class="ml-4 {finished ? 'text-accent' : 'animate-pulse-accent text-accent'}">AI</span>
      </h1>
      
      <div class="flex items-center gap-8 text-white/40 font-medium text-sm">
        <div class="flex flex-col items-center">
          <span class="micro-label !text-[8px] opacity-50 mb-1">{$t('processing.time')}</span>
          <span>{formatTime(elapsedSeconds)}</span>
        </div>
        <div class="w-px h-8 bg-white/10"></div>
        <div class="flex flex-col items-center">
          <span class="micro-label !text-[8px] opacity-50 mb-1">{$t('processing.progress_label')}</span>
          <span>{Math.round(globalProgress)}%</span>
        </div>
      </div>
    </div>

    <div class="max-w-4xl mx-auto mb-16 px-4">
      <div class="h-1 bg-white/5 rounded-full overflow-hidden relative">
        <div
          class="h-full bg-accent shadow-[0_0_20px_var(--color-accent)] transition-all duration-700 ease-out"
          style="width: {globalProgress}%;"
        ></div>
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-stretch">
      {#each steps as step, i}
        {@const stepData = $t('processing.steps')[i]}
        <div
          class="glass-card p-6 flex flex-col items-center text-center transition-all duration-500 border-white/5 relative h-[220px] justify-center
                    {step.status === 'running' ? 'border-accent/40 bg-accent/5 ring-1 ring-accent/10 scale-[1.02] z-20 shadow-[0_20px_40px_rgba(0,0,0,0.3)]' : 'z-10'}
                    {step.status === 'done' ? 'opacity-100 border-accent/20' : step.status === 'running' ? 'opacity-100' : 'opacity-30'}"
        >
          <div
            class="w-12 h-12 rounded-2xl flex items-center justify-center text-sm font-bold mb-6 transition-all duration-500
                         {step.status === 'done' ? 'bg-accent text-wave-900' : 'bg-white/5 text-white/30 border border-white/5'}"
          >
            {#if step.status === "running"}
              <div class="w-5 h-5 border-2 border-accent/20 border-t-accent rounded-full animate-spin"></div>
            {:else if step.status === "done"}
              <svg class="w-6 h-6" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"/></svg>
            {:else}
              {i + 1}
            {/if}
          </div>

          <div class="flex-1 flex flex-col">
            <h3 class="font-semibold text-white tracking-tight mb-2 text-sm uppercase tracking-widest" style="font-family: 'Geist', sans-serif;">{stepData.label}</h3>
            <p class="text-[10px] text-white/40 font-medium leading-relaxed">{stepData.sublabel}</p>
            
            {#if step.detail}
              <div class="mt-4 text-[9px] text-accent/80 font-bold uppercase tracking-widest bg-accent/10 py-1 px-3 rounded-full inline-block mx-auto">
                {step.detail}
              </div>
            {/if}
          </div>
        </div>
      {/each}
    </div>
  </div>
</div>

<style>
  @keyframes audio-wave {
    0%, 100% { height: 4px; }
    50% { height: 12px; }
  }

  @keyframes audio-progress {
    0% { transform: translateX(-100%); }
    100% { transform: translateX(400%); }
  }

  @keyframes pulse {
    0%, 100% { opacity: 1; filter: drop-shadow(0 0 5px var(--color-accent)); }
    50% { opacity: 0.7; filter: drop-shadow(0 0 15px var(--color-accent)); }
  }

  :global(.animate-pulse-accent) {
    animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
  }
</style>
