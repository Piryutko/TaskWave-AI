<script lang="ts">
  import { fade, scale } from 'svelte/transition';
  import { lang, t } from '$lib/i18n';
  
  let showStrategyModal = false;

  // const roadmapSteps = [
  //   {
  //     title: "Концептуальное демо",
  //     status: "Завершено",
  //     desc: "Создание визуального прототипа, архитектуры и систем анализа аудио в текст."
  //   },
  //   {
  //     title: "MVP & Beta",
  //     status: "В процессе",
  //     desc: "Интеграция с реальными API (Jira, Linear), поддержка множества языков и командная работа."
  //   },
  //   {
  //     title: "Масштабирование",
  //     status: "Q4 2026",
  //     desc: "Enterprise-решения, глубокая аналитика эффективности и автоматизация сложных workflow."
  //   }
  // ];
</script>

<svelte:head>
  <title>TaskWave AI — {$t('landing.hero_subtitle')}</title>
  <link rel="preconnect" href="https://fonts.googleapis.com" />
  <link
    href="https://fonts.googleapis.com/css2?family=Geist:wght@100..900&family=Inter:wght@400;500;600;700;800&family=Outfit:wght@400;700;900&display=swap"
    rel="stylesheet"
  />
</svelte:head>

<div
  class="min-h-screen flex items-center justify-center px-6 py-20 text-center relative overflow-hidden"
>
  <!-- Background patterns -->
  <div class="absolute inset-0 z-0 pointer-events-none opacity-20">
    <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[800px] h-[800px] bg-accent/10 rounded-full blur-[120px]"></div>
  </div>

  <!-- Top-Right Strategy Link - Absolute Corner -->
  <div class="absolute top-8 right-8 z-20 flex flex-col items-end gap-2">
    <button 
      on:click={() => showStrategyModal = true}
      class="flex items-center gap-2 text-[10px] font-bold text-white/40 hover:text-white transition-all uppercase tracking-[0.2em] group h-10 px-2"
    >
      <span class="relative flex h-1.5 w-1.5 overflow-visible">
        <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-accent/40 opacity-100"></span>
        <span class="relative inline-flex rounded-full h-1.5 w-1.5 bg-accent/50 transition-colors"></span>
      </span>
      {$t('common.strategy')}
    </button>

    <!-- Language Selector -->
    <div class="flex items-center gap-4 px-2">
      <button 
        on:click={() => $lang = 'ru'} 
        class="text-[9px] font-bold transition-all uppercase tracking-widest {$lang === 'ru' ? 'text-accent' : 'text-white/20 hover:text-white/40'}"
      >
        RU
      </button>
      <button 
        on:click={() => $lang = 'en'} 
        class="text-[9px] font-bold transition-all uppercase tracking-widest {$lang === 'en' ? 'text-accent' : 'text-white/20 hover:text-white/40'}"
      >
        EN
      </button>
      <button 
        on:click={() => $lang = 'jp'} 
        class="text-[9px] font-bold transition-all uppercase tracking-widest {$lang === 'jp' ? 'text-accent' : 'text-white/20 hover:text-white/40'}"
      >
        JP
      </button>
    </div>
  </div>

  <div class="max-w-4xl w-full mx-auto flex flex-col items-center relative z-20">
    <!-- Badge -->
    <div
      class="inline-flex items-center gap-3 px-5 py-2 rounded-full bg-white/5 border border-white/10 mb-12 animate-slide-up"
    >
      <div class="relative flex h-1.5 w-1.5">
        <span
          class="animate-ping absolute inline-flex h-full w-full rounded-full bg-accent opacity-75"
        ></span>
        <span class="relative inline-flex rounded-full h-1.5 w-1.5 bg-accent"
        ></span>
      </div>
      <span
        class="micro-label !text-[12px] text-white/50 font-medium tracking-[0.2em] uppercase pr-[0.2em]"
        >{$t('common.ai_generator')}</span
      >
    </div>

    <!-- Title -->
    <h1
      class="text-7xl md:text-[105px] font-semibold leading-[0.95] tracking-tight mb-4 animate-slide-up flex flex-wrap justify-center items-baseline gap-x-4"
      style="animation-delay: 0.1s; font-family: 'Geist', sans-serif;"
    >
      <span class="branding-text" data-text="TaskWave">TaskWave</span>
      <span class="text-accent">AI</span>
    </h1>

    <!-- Attribution -->
    <div class="flex flex-col items-center gap-1 mb-12 animate-slide-up opacity-60 hover:opacity-100 transition-opacity duration-700" style="animation-delay: 0.15s;">
      <span class="micro-label !text-[8px] tracking-[0.4em] text-white/60 uppercase">{$t('common.conceptual_demo')}</span>
      <a href="https://github.com/Piryutko" target="_blank" class="text-[9px] font-bold text-accent/70 hover:text-accent transition-colors tracking-widest uppercase">
        github.com/Piryutko
      </a>
    </div>

    <!-- Description -->
    <p
      class="text-white/40 text-lg md:text-xl font-medium max-w-2xl mb-16 leading-relaxed animate-slide-up"
      style="animation-delay: 0.2s;"
    >
      {$t('landing.hero_subtitle')}
    </p>

    <!-- CTA Area -->
    <div
      class="flex flex-col items-center gap-8 animate-slide-up relative z-30"
      style="animation-delay: 0.3s;"
    >
      <a href="/upload" class="btn-outline-premium"> {$t('landing.cta_button')} </a>
    </div>
  </div>
</div>

<!-- Strategy Modal -->
{#if showStrategyModal}
  <div 
    class="fixed inset-0 z-50 flex items-center justify-center p-6"
    transition:fade={{ duration: 300 }}
  >
    <!-- Backdrop -->
    <div 
      class="absolute inset-0 bg-black/90 cursor-pointer"
      on:click={() => showStrategyModal = false}
      on:keydown={(e) => e.key === 'Escape' && (showStrategyModal = false)}
      role="button"
      tabindex="0"
      aria-label="{$t('common.close_modal')}"
    ></div>

    <!-- Modal Content -->
    <div 
      class="glass-card w-full max-w-4xl bg-[#0a0a0b] border-white/10 relative z-10 overflow-hidden shadow-[0_30px_60px_rgba(0,0,0,0.8)]"
      transition:scale={{ duration: 400, start: 0.95 }}
    >
      <!-- Premium header line -->
      <div class="h-1 w-full bg-gradient-to-r from-transparent via-accent/50 to-transparent"></div>
      
      <div class="p-10">
        <div class="flex justify-between items-start mb-10">
          <div>
            <h2 class="text-3xl font-bold text-white mb-2 tracking-tighter" style="font-family: 'Geist', sans-serif;">{$t('common.strategy')}</h2>
            <p class="text-accent/60 text-[10px] font-bold uppercase tracking-[0.3em]">{$t('landing.roadmap_vision')}</p>
          </div>
          <button 
            on:click={() => showStrategyModal = false}
            class="w-10 h-10 rounded-full bg-white/5 flex items-center justify-center text-white/40 hover:bg-white/10 hover:text-white transition-all"
          >
            ✕
          </button>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12">
          <!-- Roadmap Section (Left Column) -->
          <div class="space-y-6">
            <h3 class="micro-label !text-accent opacity-80 uppercase tracking-widest text-[9px] border-l-2 border-accent pl-4">{$t('landing.roadmap_title')}</h3>
            <div class="space-y-4">
              {#each $t('landing.roadmap') as step}
                <div class="glass-card p-5 border-white/5 bg-white/[0.02] flex items-center justify-between group hover:border-accent/20 transition-all">
                  <div class="flex-1">
                    <div class="flex items-center gap-3 mb-1">
                      <span class="text-sm font-bold text-white">{step.title}</span>
                      <span class="text-[8px] px-2 py-0.5 rounded-full {step.status === 'Завершено' || step.status === 'Completed' || step.status === '完了' ? 'bg-green-500/10 text-green-500' : step.status === 'В процессе' || step.status === 'In Progress' || step.status === '進行中' ? 'bg-accent/10 text-accent animate-pulse' : 'bg-white/5 text-white/30'} uppercase font-black">
                        {step.status}
                      </span>
                    </div>
                    <p class="text-[11px] text-white/40 leading-relaxed">{step.desc}</p>
                  </div>
                </div>
              {/each}
            </div>
          </div>

          <!-- Market & Contact (Right Column) -->
          <div class="flex flex-col justify-between gap-12">
            <div class="space-y-6">
              <h3 class="micro-label !text-accent opacity-80 uppercase tracking-widest text-[9px] border-l-2 border-accent pl-4">{$t('landing.market_title')}</h3>
              <div class="grid grid-cols-1 gap-4">
                {#each $t('landing.market_data') as item}
                <div class="glass-card p-6 border-white/5 bg-white/[0.02]">
                  <p class="text-[14px] font-bold text-white mb-2 leading-tight">{item.title}</p>
                  <p class="text-[12px] text-white/40 leading-relaxed">{item.desc}</p>
                </div>
                {/each}
              </div>
            </div>

            <!-- Contact Section -->
            <div class="p-8 bg-white/[0.02] border border-white/5 rounded-3xl flex flex-col items-center gap-4">
               <span class="micro-label !text-[8px] opacity-40 uppercase tracking-[0.4em]">{$t('landing.contact')}</span>
               <a 
                 href="mailto:maximpiryutko@gmail.com" 
                 class="text-xl font-bold text-white hover:text-accent transition-colors"
                 style="font-family: 'Geist', sans-serif;"
               >
                 maximpiryutko@gmail.com
               </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
{/if}
