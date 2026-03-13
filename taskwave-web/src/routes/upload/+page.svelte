<script lang="ts">
  import { goto } from "$app/navigation";
  import { fade, slide, scale, fly } from "svelte/transition";
  import { t } from "$lib/i18n";

  let selectedMeetingId: string = "daily";
  let mode: "epics" | "list" | "hierarchy" = "hierarchy";
  let includeDodAc = true;
  let uploading = false;
  let showExplorer = false;

  const meetingIcons: Record<string, string> = {
    daily: `<svg class="w-full h-full" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>`,
    stakeholder: `<svg class="w-full h-full" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><polygon points="16.24 7.76 14.12 14.12 7.76 16.24 9.88 9.88 16.24 7.76"/></svg>`,
    analyst: `<svg class="w-full h-full" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 3v18h18"/><path d="m19 9-5 5-4-4-3 3"/></svg>`,
    review: `<svg class="w-full h-full" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m12 3-1.912 5.813a2 2 0 0 1-1.275 1.275L3 12l5.813 1.912a2 2 0 0 1 1.275 1.275L12 21l1.912-5.813a2 2 0 0 1 1.275-1.275L21 12l-5.813-1.912a2 2 0 0 1-1.275-1.275L12 3Z"/></svg>`,
    design: `<svg class="w-full h-full" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="12 2 2 7 12 12 22 7 12 2"/><polyline points="2 17 12 22 22 17"/><polyline points="2 12 12 17 22 12"/></svg>`,
    pmo: `<svg class="w-full h-full" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect width="7" height="9" x="3" y="3" rx="1"/><rect width="7" height="5" x="14" y="3" rx="1"/><rect width="7" height="9" x="14" y="12" rx="1"/><rect width="7" height="5" x="3" y="16" rx="1"/></svg>`
  };

  $: meetings = [
    {
      id: "daily",
      type: $t('common.sync'),
      title: $t('meetings.daily.title'),
      desc: $t('meetings.daily.desc'),
      duration: $t('common.15min'),
      attendees: 8,
      size: "42 MB",
      date: $t('common.today_1030'),
      color: "from-blue-400/20 to-transparent"
    },
    {
      id: "stakeholder",
      type: $t('common.strategic'),
      title: $t('meetings.stakeholder.title'),
      desc: $t('meetings.stakeholder.desc'),
      duration: $t('common.45min'),
      attendees: 3,
      size: "128 MB",
      date: $t('common.yesterday_1415'),
      color: "from-purple-400/20 to-transparent"
    },
    {
      id: "analyst",
      type: $t('common.research'),
      title: $t('meetings.analyst.title'),
      desc: $t('meetings.analyst.desc'),
      duration: $t('common.30min'),
      attendees: 2,
      size: "86 MB",
      date: $t('common.march_12'),
      color: "from-emerald-400/20 to-transparent"
    },
    {
      id: "review",
      type: $t('common.review'),
      title: $t('meetings.review.title'),
      desc: $t('meetings.review.desc'),
      duration: $t('common.25min'),
      attendees: 12,
      size: "64 MB",
      date: $t('common.march_10'),
      color: "from-amber-400/20 to-transparent"
    },
    {
      id: "design",
      type: $t('common.design_review_type'),
      title: $t('meetings.design.title'),
      desc: $t('meetings.design.desc'),
      duration: $t('common.40min'),
      attendees: 2,
      size: "112 MB",
      date: $t('common.today_0900'),
      color: "from-pink-400/20 to-transparent"
    },
    {
      id: "pmo",
      type: $t('common.management'),
      title: $t('meetings.pmo.title'),
      desc: $t('meetings.pmo.desc'),
      duration: $t('common.60min'),
      attendees: 5,
      size: "156 MB",
      date: $t('common.yesterday_1630'),
      color: "from-indigo-400/20 to-transparent"
    }
  ];

  $: selectedMeeting = meetings.find(m => m.id === selectedMeetingId) || meetings[0];

  async function submit() {
    uploading = true;
    await new Promise(r => setTimeout(r, 1500));
    goto(`/processing/${selectedMeetingId}?mode=${mode}&dod=${includeDodAc}`);
  }

  function handleFileSelect(id: string) {
    selectedMeetingId = id;
    setTimeout(() => { showExplorer = false; }, 300);
  }
</script>

<svelte:head>
  <title>{$t('upload.library_title')} — TaskWave AI</title>
</svelte:head>

<div class="h-screen w-full flex flex-col items-center justify-center px-6 overflow-hidden">
  
  <!-- Back Button (Elegant & Minimal) -->
  <div class="fixed top-8 left-8 z-[50]" in:fade={{ delay: 500, duration: 800 }}>
    <button 
      on:click={() => goto('/')}
      class="text-white/50 hover:text-accent transition-all duration-300 group/back py-2"
      aria-label="{$t('common.back')}"
    >
      <div class="w-8 h-8 rounded-full border border-white/20 flex items-center justify-center group-hover/back:border-accent/40 group-hover/back:bg-accent/10 group-hover/back:shadow-[0_0_15px_rgba(212,163,115,0.2)] transition-all">
        <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m15 18-6-6 6-6"/></svg>
      </div>
    </button>
  </div>

  <!-- Header Minimal -->
  <div class="mb-12 flex flex-col items-center" in:fade={{ duration: 800 }}>
    <h1 class="text-2xl font-semibold text-white tracking-tight uppercase tracking-[0.2em] opacity-80" style="font-family: 'Geist', sans-serif;">
      {$t('upload.config_title')}
    </h1>
  </div>

  <div class="w-full max-w-6xl grid grid-cols-1 lg:grid-cols-3 gap-6 items-stretch">
    
    <!-- Column 1: Audio Selection -->
    <div 
      class="glass-card p-8 flex flex-col justify-center border-white/5 group" 
      in:fly={{ y: 20, duration: 800, delay: 100 }}
    >
      <h3 class="micro-label mb-8 text-white/40">{$t('upload.step_1')}</h3>
      
      <div class="flex flex-col items-center text-center">
        <!-- Selected File Preview -->
        <button 
          on:click={() => showExplorer = true}
          class="w-24 h-24 rounded-3xl bg-white/5 border border-white/10 flex items-center justify-center mb-6 hover:border-accent/40 hover:bg-white/[0.08] transition-all duration-500 relative overflow-hidden group/preview"
        >
          <div class="absolute inset-0 bg-gradient-to-br {selectedMeeting.color} opacity-20"></div>
          <div class="w-10 h-10 text-white/80 z-10 transition-transform duration-500 group-hover/preview:scale-110">
            {@html meetingIcons[selectedMeeting.id]}
          </div>
        </button>

        <p class="text-white font-medium text-lg mb-1" style="font-family: 'Geist', sans-serif;">{selectedMeeting.title}</p>
        <p class="text-white/20 text-[10px] uppercase tracking-widest mb-8">{selectedMeeting.size} • {selectedMeeting.duration}</p>

        <button 
          on:click={() => showExplorer = true}
          class="btn-outline-premium !py-3 !px-8 !text-[11px] !border-white/10 hover:!border-accent/40 bg-white/5"
        >
          {$t('upload.open_library')}
        </button>
      </div>
    </div>

    <!-- Column 2: Analysis Configuration -->
    <div 
      class="glass-card p-8 border-white/5" 
      in:fly={{ y: 20, duration: 800, delay: 200 }}
    >
      <h3 class="micro-label mb-6 text-white/40">{$t('upload.step_2')}</h3>
      
      <div class="space-y-8">
        <div>
          <p class="text-[10px] text-white/30 uppercase tracking-[0.2em] mb-4">{$t('upload.mode_label')}</p>
          <div class="flex flex-col gap-2">
            {#each [
              {id:'epics', label:$t('result.epics')}, 
              {id:'list', label:$t('result.list')}, 
              {id:'hierarchy', label:$t('result.hierarchy')}
            ] as m}
              <button 
                on:click={() => mode = m.id as any}
                class="w-full py-3 px-4 rounded-xl border text-[11px] font-semibold uppercase tracking-widest transition-all text-left flex justify-between items-center
                       {mode === m.id ? 'border-accent bg-accent/5 text-white' : 'border-white/5 text-white/30 hover:border-white/20'}"
              >
                {m.label}
                {#if mode === m.id}
                  <div class="w-1.5 h-1.5 rounded-full bg-accent"></div>
                {/if}
              </button>
            {/each}
          </div>
        </div>

        <div>
          <div class="flex items-center justify-between mb-2">
            <p class="text-[10px] text-white/30 uppercase tracking-[0.2em]">{$t('upload.details_label')}</p>
            <label class="relative inline-flex items-center cursor-pointer">
              <input type="checkbox" class="sr-only peer" bind:checked={includeDodAc} />
              <div class="w-10 h-5 bg-white/5 rounded-full peer peer-checked:after:translate-x-5 peer-checked:after:border-white after:content-[''] after:absolute after:top-1 after:left-1 after:bg-white after:border-white after:border after:rounded-full after:h-3 after:w-3 after:transition-all peer-checked:bg-accent/60 transition-colors"></div>
            </label>
          </div>
          <p class="text-[10px] text-white/20 leading-relaxed">{$t('upload.details_desc')}</p>
        </div>
      </div>
    </div>

    <!-- Column 3: Summary & Action -->
    <div 
      class="glass-card p-0 overflow-hidden border-accent/20 bg-accent/5 flex flex-col" 
      in:fly={{ y: 20, duration: 800, delay: 300 }}
    >
      <div class="p-8 flex-1">
        <h3 class="micro-label mb-6 text-white/40">{$t('upload.step_3')}</h3>
        
        <div class="mb-6 h-20 flex flex-col justify-center">
          <div>
          <p class="text-2xl font-semibold text-white mb-2 leading-tight" style="font-family: 'Geist', sans-serif;">
            {$t('upload.ready_to_analyze')}
          </p>
          <p class="text-white/40 text-[13px] leading-relaxed">
            {$t('upload.analyze_desc', mode === 'hierarchy' ? $t('result.hierarchy') : mode === 'epics' ? $t('result.epics') : $t('result.list'), includeDodAc)}
          </p>
        </div>
      </div>

        <div class="grid grid-cols-2 gap-4 border-t border-white/5 pt-6">
          <div>
            <p class="text-[9px] text-white/20 uppercase tracking-widest mb-1">{$t('upload.duration')}</p>
            <p class="text-white text-sm font-medium">{selectedMeeting.duration}</p>
          </div>
          <div>
            <p class="text-[9px] text-white/20 uppercase tracking-widest mb-1">{$t('common.attendees')}</p>
            <p class="text-white text-sm font-medium">{selectedMeeting.attendees} {$t('upload.attendees')}</p>
          </div>
        </div>
      </div>

      <button 
        class="btn-outline-premium !rounded-none !border-0 !border-t !border-accent/30 !py-8 bg-accent/10 hover:bg-accent/20
               {uploading ? 'opacity-50 pointer-events-none' : ''}"
        on:click={submit}
      >
        {#if uploading}
          <span class="animate-pulse tracking-[0.2em]">{$t('upload.analyzing')}</span>
        {:else}
          {$t('upload.start_btn')}
        {/if}
      </button>
    </div>

  </div>

</div>

<!-- SIMULATED FILE EXPLORER MODAL -->
{#if showExplorer}
  <div 
    class="fixed inset-0 z-[100] flex items-center justify-center p-6 bg-black/80 backdrop-blur-sm"
    transition:fade={{ duration: 300 }}
    on:mousedown={() => showExplorer = false}
    aria-hidden="true"
  >
    <div 
      class="w-full max-w-4xl h-[600px] glass-card border-white/10 flex flex-col overflow-hidden animate-scale-up"
      on:mousedown|stopPropagation
      role="dialog"
      aria-modal="true"
      tabindex="-1"
    >
      <!-- Explorer Title Bar -->
      <div class="p-4 border-b border-white/10 flex items-center justify-between bg-white/[0.02]">
        <div class="flex items-center gap-4">
          <div class="flex gap-1.5 mr-4">
            <div class="w-3 h-3 rounded-full bg-white/10"></div>
            <div class="w-3 h-3 rounded-full bg-white/10"></div>
            <div class="w-3 h-3 rounded-full bg-white/10"></div>
          </div>
          <div class="flex items-center gap-2 text-[11px] text-white/40 uppercase tracking-widest font-medium">
            <span>CloudWave</span>
            <span class="text-white/10">/</span>
            <span class="text-accent/60">TaskWave AI</span>
            <span class="text-white/10">/</span>
            <span>{$t('upload.explorer.title')}</span>
          </div>
        </div>
        <button 
          on:click={() => showExplorer = false}
          class="text-white/20 hover:text-white transition-colors p-2"
        >
          ✕
        </button>
      </div>

      <div class="flex-1 flex overflow-hidden">
        <!-- Sidebar -->
        <div class="w-48 border-r border-white/10 p-6 space-y-8 bg-white/[0.01]">
          <div>
            <p class="text-[9px] text-white/20 uppercase tracking-widest mb-4">{$t('common.main')}</p>
            <div class="space-y-3">
              {#each [
                { id: 'recent', label: $t('upload.explorer.recent') },
                { id: 'favorites', label: $t('upload.explorer.favorites') },
                { id: 'cloud', label: 'CloudWave' }
              ] as item}
                <div class="text-[11px] text-white/40 hover:text-white cursor-pointer py-1 flex items-center gap-2 transition-colors">
                  <span class="w-3.5 h-3.5 opacity-30">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"/></svg>
                  </span>
                  {item.label}
                </div>
              {/each}
            </div>
          </div>

          <div>
            <p class="text-[9px] text-white/20 uppercase tracking-widest mb-4">{$t('upload.explorer.projects')}</p>
            <div class="space-y-3">
              {#each ['TimeWave AI', 'ToneWave AI', 'TaskWave AI'] as item}
                <div class="text-[11px] {item === 'TaskWave AI' ? 'text-accent font-semibold' : 'text-white/40'} hover:text-white cursor-pointer py-1 flex items-center gap-2 transition-colors">
                  <span class="w-3.5 h-3.5 opacity-30">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"/></svg>
                  </span>
                  {item}
                </div>
              {/each}
            </div>
          </div>
        </div>

        <!-- Main Content (Audio Files) -->
        <div class="flex-1 p-8 overflow-y-auto custom-scrollbar">
          <div class="grid grid-cols-2 md:grid-cols-3 gap-6">
            {#each meetings as m}
              <button 
                on:click={() => handleFileSelect(m.id)}
                class="flex flex-col items-center p-6 rounded-2xl border transition-all duration-300 group
                       {selectedMeetingId === m.id 
                         ? 'bg-accent/10 border-accent/40 ring-1 ring-accent/20' 
                         : 'bg-white/[0.02] border-white/5 hover:border-white/20 hover:bg-white/[0.04]'}"
              >
                <div class="w-12 h-12 mb-4 flex items-center justify-center transition-transform group-hover:scale-110 duration-500 text-white/60 group-hover:text-accent font-light">
                  {@html meetingIcons[m.id]}
                </div>
                <p class="text-white font-medium text-xs text-center mb-1 leading-snug">{m.title}</p>
                <p class="text-[9px] text-white/20 uppercase tracking-tight">{m.date} • {m.size}</p>
              </button>
            {/each}
          </div>
        </div>
      </div>
    </div>
  </div>
{/if}

<style>
  @keyframes audio-wave {
    0%, 100% { height: 4px; }
    50% { height: 12px; }
  }

  @keyframes audio-progress {
    0% { transform: translateX(-100%); }
    100% { transform: translateX(400%); }
  }

  @keyframes scaleUp {
    from { opacity: 0; transform: scale(0.95); }
    to { opacity: 1; transform: scale(1); }
  }

  .animate-scale-up {
    animation: scaleUp 0.4s cubic-bezier(0.16, 1, 0.3, 1) forwards;
  }

  .custom-scrollbar::-webkit-scrollbar {
    width: 3px;
  }
  .custom-scrollbar::-webkit-scrollbar-track {
    background: transparent;
  }
  .custom-scrollbar::-webkit-scrollbar-thumb {
    background: rgba(255, 255, 255, 0.05);
    border-radius: 10px;
  }
  .custom-scrollbar::-webkit-scrollbar-thumb:hover {
    background: rgba(212, 163, 115, 0.2);
  }

  :global(.animate-fade-in) {
    animation: fadeIn 0.8s ease-out forwards;
  }
  
  @keyframes fadeIn {
    from { opacity: 0; }
    to { opacity: 1; }
  }
</style>
