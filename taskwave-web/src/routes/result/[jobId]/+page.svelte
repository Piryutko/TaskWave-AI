<script lang="ts">
  import { page } from "$app/stores";
  import { onMount } from "svelte";
  import { t } from "$lib/i18n";
  import { mockAudioFiles } from "$lib/mockData";
  import { fade, slide, scale, fly } from "svelte/transition";
  import { cubicOut } from "svelte/easing";
  import { goto } from "$app/navigation";

  const jobId = $page.params.jobId || "session-id";
  const modeParam = $page.url.searchParams.get("mode") || "hierarchy";
  const includeDod = $page.url.searchParams.get("dod") !== "false";

  let viewMode: "hierarchy" | "list" | "epics" =
    (modeParam as any) === "epics"
      ? "epics"
      : modeParam === "list"
        ? "list"
        : "hierarchy";
  let loading = true;
  let exporting = false;
  let exportSuccess = false;
  let activeTracker = "";
  let showExportModal = false;
  let exportSimStep: "config" | "syncing" | "success" = "config";
  let syncProgress = 0;
  let syncStatusText = "";
  
  let selectedProject = "TaskWave AI";
  let showProjectDropdown = false;
  let selectedStatus = "Backlog";

  const rawJobType = $page.url.searchParams.get("jobType") || jobId.replace("-job", "");
  // Fallback to daily-standup if jobType is unknown
  const jobType = mockAudioFiles[rawJobType] ? rawJobType : "daily-standup";
  
  // Localized epics data
  $: epics = $t(`projects.${jobType}`) as any[] || [];

  onMount(async () => {
    await new Promise((r) => setTimeout(r, 1200)); // Atmos loading
    loading = false;
  });

  function toggleTask(epicIdx: number, taskIdx: number) {
    epics[epicIdx].tasks[taskIdx].expanded =
      !epics[epicIdx].tasks[taskIdx].expanded;
    epics = [...epics];
  }

  function openExportModal(tracker: string) {
    activeTracker = tracker;
    exportSimStep = "config";
    syncProgress = 0;
    showExportModal = true;
  }

  async function startFinalExport() {
    exporting = true;
    exportSimStep = "syncing";
    
    const taskCount = epics.reduce((a, b) => a + b.tasks.length, 0);
    const steps = [
      { p: 10, text: $t('result.modal.sync_steps.0', activeTracker.toUpperCase()) },
      { p: 30, text: $t('result.modal.sync_steps.1') },
      { p: 60, text: $t('result.modal.sync_steps.2', taskCount) },
      { p: 85, text: $t('result.modal.sync_steps.3') },
      { p: 100, text: $t('result.modal.sync_steps.4') }
    ];

    for (const step of steps) {
      syncStatusText = step.text;
      syncProgress = step.p;
      await new Promise(r => setTimeout(r, 800 + Math.random() * 600));
    }

    exportSimStep = "success";
    exportSuccess = true;
    exporting = false;
    
    setTimeout(() => {
      showExportModal = false;
      exportSuccess = false;
      activeTracker = "";
    }, 2000);
  }


  const trackerIcons: Record<string, string> = {
    linear: `<svg viewBox="0 0 24 24" fill="currentColor" class="w-5 h-5"><path d="M21.328 17.556L12 2 2.672 17.556H21.328zM12 6.66l5.444 9.111H6.556L12 6.66z"/></svg>`,
    jira: `<svg viewBox="0 0 24 24" fill="currentColor" class="w-5 h-5"><path d="M11.53,2C11.83,2,12.13,2.15,12.33,2.4L21.73,11.8C22.03,12.1,22.03,12.6,21.73,12.9L12.33,22.3C12.13,22.5,11.83,22.6,11.53,22.6C11.13,22.6,10.83,22.3,10.83,21.9V12.7H2.13C1.73,12.7,1.43,12.4,1.43,12C1.43,11.7,1.58,11.5,1.78,11.4L10.83,2.3V2.4C10.83,2.1,11.13,2,11.53,2Z"/></svg>`,
    trello: `<svg viewBox="0 0 24 24" fill="currentColor" class="w-5 h-5"><path d="M19,3H5C3.9,3,3,3.9,3,5v14c0,1.1,0.9,2,2,2h14c1.1,0,2-0.9,2-2V5C21,3.9,20.1,3,19,3z M10,17H7V7h3V17z M17,13h-3V7h3V13z"/></svg>`,
  };
</script>

<svelte:head>
  <title>{$t('result.title')} — TaskWave AI</title>
</svelte:head>

<div class="min-h-screen w-full bg-wave-900 flex flex-col pt-12">
  <div class="max-w-7xl mx-auto w-full px-6 flex-1 flex flex-col">
    {#if loading}
      <div
        class="flex-1 flex flex-col items-center justify-center gap-6"
        in:fade
      >
        <div
          class="w-16 h-16 border-2 border-accent/10 border-t-accent rounded-full animate-spin"
        ></div>
        <p class="micro-label !text-[10px] animate-pulse">
          {$t('result.formalizing')}
        </p>
      </div>
    {:else}
      <!-- Header Area -->
      <div
        class="flex flex-col md:flex-row items-end justify-between gap-8 mb-16 relative z-10"
        in:fly={{ y: -20, duration: 800 }}
      >
        <div class="flex flex-col gap-4">
          <div class="flex items-center gap-3">
            <span class="w-2 h-2 rounded-full bg-accent animate-pulse"></span>
            <span class="micro-label !text-[10px] text-accent tracking-[0.3em]"
              >{$t('result.analysis_ready')}</span
            >
          </div>
          <h1
            class="text-5xl md:text-6xl font-bold text-white tracking-tighter"
            style="font-family: 'Geist', sans-serif;"
          >
            TaskWave <span class="text-accent animate-pulse-accent">AI</span>
          </h1>
          <p class="text-white/30 text-sm font-medium">
            {$t('result.session_name')}
          </p>
        </div>

        <div class="flex flex-col items-end gap-6">
          <div
            class="flex items-center gap-3 px-4 py-2 bg-white/5 rounded-2xl border border-white/5"
          >
            <span
              class="micro-label !text-[8px] opacity-40 uppercase tracking-widest"
              >{$t('upload.mode_label')}:</span
            >
            <span
              class="text-[10px] font-bold text-accent uppercase tracking-[0.2em]"
            >
              {$t(`result.${viewMode}`)}
            </span>
          </div>
        </div>
      </div>

      <!-- Main Visual Area -->
      <div class="flex-1 mb-12 relative">
        {#if viewMode === "hierarchy" || viewMode === "epics"}
          <div
            class="flex overflow-x-auto pt-4 pb-8 gap-6 custom-scrollbar hide-scrollbar justify-center {viewMode === 'epics' ? 'items-stretch' : 'items-start'}"
            in:fade
          >
            {#each epics as epic, epicIdx}
              <div
                class="flex-shrink-0 w-[380px] flex flex-col gap-6 relative z-20"
                in:fly={{ x: 30, delay: epicIdx * 100 }}
              >
                <!-- Epic Card -->
                {#if viewMode === "epics"}
                  <!-- Premium Hero Frame for Epics Mode - Compact Version -->
                  <div
                    class="glass-card w-full h-[420px] p-8 border-accent/40 bg-accent/5 relative group hover:z-30 transition-all overflow-hidden flex flex-col items-center text-center"
                  >
                    <div class="w-12 h-1 bg-accent/30 mb-8 rounded-full"></div>
                    <h2
                      class="text-2xl font-bold text-white mb-4 leading-tight max-w-full"
                      style="font-family: 'Geist', sans-serif;"
                    >
                      {epic.title}
                    </h2>
                    <p
                      class="text-white/40 text-[13px] leading-relaxed mb-6 max-w-[300px]"
                    >
                      {epic.desc}
                    </p>
                    <div
                      class="flex items-center gap-3 text-[9px] font-bold text-white/20 uppercase tracking-widest mt-auto border-t border-white/5 pt-5 w-full justify-center"
                    >
                      <span>TaskWave AI</span>
                    </div>
                  </div>
                {:else}
                  <!-- Classic Hub Frame for Hierarchy Mode -->
                  <div
                    class="glass-card w-full h-[160px] p-6 border-accent/20 bg-accent/5 relative group hover:z-30 transition-all"
                  >
                    <div class="micro-label !text-[9px] mb-4 opacity-50">
                      {$t('result.epic_label')} 0{epicIdx + 1}
                    </div>
                    <h2
                      class="text-xl font-bold text-white mb-2 leading-tight"
                      style="font-family: 'Geist', sans-serif;"
                    >
                      {epic.title}
                    </h2>
                    <div class="w-full h-px bg-white/5 mt-4"></div>

                    <!-- Connection Point -->
                    <div
                      class="absolute -bottom-6 left-1/2 w-px h-6 bg-accent/30 group-hover:bg-accent/60 transition-colors"
                    ></div>
                  </div>
                {/if}

                <!-- Tasks under Epic -->
                {#if viewMode === "hierarchy"}
                  <div class="flex flex-col gap-4 relative">
                    <!-- Main vertical line -->
                    <div
                      class="absolute top-0 bottom-0 left-1/2 w-px bg-accent/20 -z-10"
                    ></div>

                    {#each epic.tasks as task, taskIdx}
                      <div class="relative group/task">
                        <!-- Horizontal connector / Anchor Dot -->
                        <div
                          class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-1.5 h-1.5 rounded-full bg-accent/30 -z-10 group-hover/task:bg-accent transition-colors"
                        ></div>

                        <button
                          on:click={() => toggleTask(epicIdx, taskIdx)}
                          class="w-full glass-card p-5 text-left border-white/5 hover:border-accent/40 transition-all duration-300 relative
                                 {task.expanded
                            ? 'bg-wave-800/90 border-accent/30 shadow-[0_10px_30px_rgba(0,0,0,0.2)]'
                            : 'bg-wave-900/60 backdrop-blur-xl hover:bg-wave-800/40'}"
                        >
                          <div class="flex items-start justify-between gap-4">
                            <div class="flex-1">
                              <h3
                                class="text-sm font-semibold text-white/90 mb-1 leading-snug"
                              >
                                {task.title}
                              </h3>
                              {#if !task.expanded}
                                <p
                                  class="text-[10px] text-white/30 line-clamp-1"
                                >
                                  {task.desc}
                                </p>
                              {/if}
                            </div>
                            <div
                              class="w-6 h-6 rounded-lg bg-white/5 flex items-center justify-center text-[10px] text-white/40 group-hover/task:bg-accent group-hover/task:text-wave-900 transition-all"
                            >
                              {task.expanded ? "−" : "+"}
                            </div>
                          </div>

                          {#if task.expanded}
                            <div
                              class="mt-4 space-y-4"
                              transition:slide={{
                                duration: 400,
                                easing: cubicOut,
                              }}
                            >
                              <p
                                class="text-[11px] text-white/50 leading-relaxed font-medium"
                              >
                                {task.desc}
                              </p>

                              <div
                                class="grid grid-cols-1 gap-4 pt-4 border-t border-white/5"
                              >
                                {#if task.ac && task.ac.length > 0}
                                  <div>
                                    <p class="micro-label !text-[8px] mb-2 text-accent/80">
                                      {$t('result.ac_label')}
                                    </p>
                                    <ul class="space-y-1.5">
                                      {#each task.ac as item}
                                        <li class="flex items-center gap-2 text-[10px] text-white/80">
                                          <span class="w-1 h-1 rounded-full bg-accent"></span>
                                          {item}
                                        </li>
                                      {/each}
                                    </ul>
                                  </div>
                                {/if}

                                <div>
                                  <p class="micro-label !text-[8px] mb-2 opacity-50">
                                    {$t('result.dod_label')}
                                  </p>
                                  <ul class="space-y-1.5">
                                    {#each task.dod as item}
                                      <li
                                        class="flex items-center gap-2 text-[10px] text-white/60"
                                      >
                                        <span
                                          class="w-1 h-1 rounded-full bg-white/20"
                                        ></span>
                                        {item}
                                      </li>
                                    {/each}
                                  </ul>
                                </div>
                              </div>
                            </div>
                          {/if}
                        </button>
                      </div>
                    {/each}
                  </div>
                {/if}
              </div>
            {/each}
          </div>
        {:else}
          <div class="max-w-4xl mx-auto w-full space-y-4" in:fade>
            {#each epics as epic, epicIdx}
              {#each epic.tasks as task, taskIdx}
                <div class="flex flex-col gap-1">
                  <div
                    class="glass-card p-6 border-white/5 hover:border-white/10 transition-all group flex items-center justify-between gap-8 {task.expanded
                      ? 'bg-white/5 border-accent/20'
                      : ''}"
                  >
                    <div class="flex-1">
                      <div class="flex items-center gap-3 mb-2">
                        <span
                          class="micro-label !text-[8px] px-2 py-0.5 bg-white/5 rounded-full text-white/40"
                          >{epic.title}</span
                        >
                        {#if task.expanded}
                          <span
                            class="micro-label !text-[8px] px-2 py-0.5 bg-accent/10 rounded-full text-accent animate-fade-in"
                            >{$t('result.active_badge')}</span
                          >
                        {/if}
                      </div>
                      <h3
                        class="text-lg font-semibold text-white group-hover:text-accent transition-colors"
                      >
                        {task.title}
                      </h3>
                    </div>
                    <button
                      on:click={() => toggleTask(epicIdx, taskIdx)}
                      class="btn-outline-premium !py-2 !px-6 !text-[10px] !border-white/10 group-hover:!border-accent/40"
                    >
                      {task.expanded ? $t('result.hide_btn') : $t('result.details_btn')}
                    </button>
                  </div>

                  {#if task.expanded}
                    <div
                      class="glass-card p-8 border-t-0 border-white/5 bg-white/[0.01] rounded-t-none -mt-1 mx-2"
                      transition:slide
                    >
                      <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                        <div class="space-y-6">
                          <div>
                            <p class="micro-label !text-[8px] mb-2 opacity-40">
                              {$t('result.desc_label')}
                            </p>
                            <p
                              class="text-[13px] text-white/70 leading-relaxed font-medium"
                            >
                              {task.desc}
                            </p>
                          </div>

                          <div
                            class="p-4 bg-white/5 rounded-2xl border border-white/5"
                          >
                            <p class="micro-label !text-[8px] mb-2 opacity-40">
                              {$t('result.source_label')}
                            </p>
                            <div class="flex items-center gap-3">
                              <div
                                class="w-8 h-8 rounded-full bg-accent/20 flex items-center justify-center text-accent"
                              >
                                <svg
                                  class="w-4 h-4"
                                  viewBox="0 0 24 24"
                                  fill="none"
                                  stroke="currentColor"
                                  stroke-width="2"
                                  ><path
                                    d="M12 1a3 3 0 0 0-3 3v8a3 3 0 0 0 6 0V4a3 3 0 0 0-3-3z"
                                  /><path
                                    d="M19 10v2a7 7 0 0 1-14 0v-2"
                                  /><line x1="12" y1="19" x2="12" y2="23" /><line
                                    x1="8"
                                    y1="23"
                                    x2="16"
                                    y2="23"
                                  /></svg
                                >
                              </div>
                              <div class="flex flex-col">
                                <span
                                  class="text-[10px] font-bold text-white uppercase tracking-wider"
                                  >{jobType.replace("-", " ")}</span
                                >
                                <span
                                  class="text-[8px] text-white/30 truncate max-w-[150px]"
                                  >{mockAudioFiles[jobType] || mockAudioFiles['daily-standup']}</span
                                >
                              </div>
                            </div>
                          </div>
                        </div>

                        <div class="space-y-6">
                            <div class="space-y-4">
                              {#if task.ac && task.ac.length > 0}
                                <div>
                                  <p
                                    class="micro-label !text-[8px] mb-2 text-accent/80"
                                  >
                                    {$t('result.ac_label')}
                                  </p>
                                  <ul class="space-y-1.5">
                                    {#each task.ac as item}
                                      <li
                                        class="flex items-center gap-2 text-[10px] text-white/80"
                                      >
                                        <span
                                          class="w-1 h-1 rounded-full bg-accent"
                                        ></span>
                                        {item}
                                      </li>
                                    {/each}
                                  </ul>
                                </div>
                              {/if}

                              <div>
                                <p
                                  class="micro-label !text-[8px] mb-2 opacity-50"
                                >
                                  {$t('result.dod_label')}
                                </p>
                                <ul class="space-y-1.5">
                                  {#each task.dod as item}
                                    <li
                                      class="flex items-center gap-2 text-[10px] text-white/60"
                                    >
                                      <span
                                        class="w-1 h-1 rounded-full bg-white/20"
                                      ></span>
                                      {item}
                                    </li>
                                  {/each}
                                </ul>
                              </div>
                            </div>
                        </div>
                      </div>
                    </div>
                  {/if}
                </div>
              {/each}
            {/each}
          </div>
        {/if}
      </div>

      <!-- Action Bar (Sticky Bottom style) -->
      <div class="mt-auto pb-12" in:fly={{ y: 20, duration: 800, delay: 400 }}>
        <div
          class="glass-card p-6 border-white/10 bg-white/[0.02] flex flex-col md:flex-row items-center justify-between gap-8"
        >
          <div class="flex flex-col gap-2">
            <h4 class="micro-label !text-[10px]">{$t('result.export_trackers_label')}</h4>
            <div class="flex items-center gap-3">
              {#each ["linear", "jira", "trello"] as tracker}
                <button
                  on:click={() => openExportModal(tracker)}
                  disabled={exporting}
                  class="w-12 h-12 rounded-2xl flex items-center justify-center transition-all duration-300 relative
                         {activeTracker === tracker
                    ? 'bg-accent/20 border border-accent/40 shadow-[0_0_20px_rgba(212,163,115,0.2)]'
                    : 'bg-white/5 text-white/30 hover:bg-white/10 hover:text-white'}
                         {exporting && activeTracker !== tracker
                    ? 'opacity-30'
                    : ''}"
                  title="{$t('common.strategy')} {tracker}"
                >
                  {@html trackerIcons[tracker]}
                </button>
              {/each}

              {#if exportSuccess}
                <div
                  class="ml-4 flex items-center gap-2 text-accent font-bold text-[10px] uppercase tracking-widest animate-fade-in"
                  in:fly={{ x: -10 }}
                >
                  <svg
                    class="w-4 h-4"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="3"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    ><polyline points="20 6 9 17 4 12" /></svg
                  >
                  {$t('result.export_success_msg')} {activeTracker}
                </div>
              {/if}
            </div>
          </div>

          <div class="flex items-center gap-4">
            <a
              href="/upload"
              class="text-[10px] font-bold text-white/20 hover:text-white uppercase tracking-widest transition-colors"
            >
              {$t('result.new_tasks_link')}
            </a>
          </div>
        </div>
      </div>
    {/if}
  </div>
</div>

<!-- Export Modal Simulation -->
{#if showExportModal}
  <div class="fixed inset-0 z-[100] flex items-center justify-center p-6" transition:fade={{ duration: 200 }}>
    <!-- Backdrop -->
    <div 
      class="absolute inset-0 bg-black/90" 
      role="button"
      tabindex="-1"
      on:click={() => !exporting && (showExportModal = false)}
      on:keydown={(e) => e.key === 'Escape' && !exporting && (showExportModal = false)}
    ></div>
    
    <!-- Modal Content -->
    <div 
      class="w-full max-w-lg glass-card p-10 relative z-10 border-white/10 overflow-hidden"
      in:fade={{ duration: 200 }}
    >
      {#if exportSimStep === "config"}
        <div class="flex flex-col items-center text-center gap-6">
          <div class="w-16 h-16 rounded-2xl bg-white/5 flex items-center justify-center mb-2 shadow-inner">
            {@html trackerIcons[activeTracker]}
          </div>
          <div>
            <h2 class="text-2xl font-bold text-white mb-2 uppercase tracking-tight">{$t('result.modal.title')} {activeTracker}</h2>
            <p class="text-white/40 text-sm">{$t('result.modal.subtitle')}</p>
          </div>

          <div class="w-full space-y-8 mt-4">
            <!-- Project Selection Dropdown -->
            <div class="text-left space-y-3 relative">
              <span class="micro-label !text-[9px] opacity-40 ml-1">{$t('result.modal.target_project')}</span>
              <button 
                type="button"
                on:click={() => (showProjectDropdown = !showProjectDropdown)}
                class="w-full p-4 bg-white/5 rounded-2xl border border-white/10 text-white/80 text-sm font-medium flex items-center justify-between hover:border-accent/40 hover:bg-white/[0.08] transition-all"
              >
                <span>{selectedProject}</span>
                <span class="text-[10px] text-accent/50 transition-transform {showProjectDropdown ? 'rotate-180' : ''}">▼</span>
              </button>
              
              {#if showProjectDropdown}
                <div 
                  class="absolute top-full left-0 right-0 mt-2 p-2 glass-card bg-[#0a0a0b] border-white/20 z-50 shadow-[0_20px_40px_rgba(0,0,0,0.6)] overflow-hidden"
                  transition:fade={{ duration: 150 }}
                >
                  {#each ["TaskWave AI", "ToneWave AI", "TimeWave AI"] as project}
                    <button 
                      type="button"
                      on:click={() => { selectedProject = project; showProjectDropdown = false; }}
                      class="w-full p-3 text-left rounded-xl text-sm font-medium transition-all hover:bg-accent/10 {selectedProject === project ? 'text-accent bg-accent/5' : 'text-white/60 hover:text-white'}"
                    >
                      {project}
                    </button>
                  {/each}
                </div>
              {/if}
            </div>

            <!-- Status Selection Chips -->
            <div class="text-left space-y-4 mt-6">
              <span class="micro-label !text-[9px] opacity-40 ml-1 block mb-1">{$t('result.modal.target_status')}</span>
              <div class="flex flex-wrap gap-2">
                {#each ["Backlog", "Sprint Planning", "In Review"] as status}
                  <button 
                    type="button"
                    on:click={() => (selectedStatus = status)}
                    class="px-5 py-3 rounded-xl border transition-all text-[11px] font-bold uppercase tracking-wider
                           {selectedStatus === status 
                             ? 'bg-accent/20 border-accent/60 text-white shadow-[0_0_15px_rgba(212,163,115,0.2)]' 
                             : 'bg-white/5 border-white/10 text-white/40 hover:border-white/20 hover:text-white/60'}"
                  >
                    {status}
                  </button>
                {/each}
              </div>
            </div>
          </div>

          <button 
            on:click={startFinalExport}
            class="w-full mt-6 btn-outline-premium !py-4 !text-[12px] group"
          >
            {$t('result.modal.confirm')}
          </button>
        </div>
      {:else if exportSimStep === "syncing"}
        <div class="flex flex-col items-center text-center gap-8 py-10" in:fade>
          <div class="relative w-24 h-24">
            <div class="absolute inset-0 border-4 border-accent/10 rounded-full"></div>
            <div 
              class="absolute inset-0 border-4 border-accent rounded-full animate-spin border-t-transparent"
              style="animation-duration: 0.8s"
            ></div>
            <div class="absolute inset-0 flex items-center justify-center opacity-40">
              {@html trackerIcons[activeTracker]}
            </div>
          </div>
          
          <div class="flex flex-col items-center gap-3 w-full">
            <p class="text-[12px] font-bold text-white uppercase tracking-[0.3em] w-full">{syncStatusText}</p>
            <div class="w-64 h-1 bg-white/5 rounded-full overflow-hidden mx-auto">
              <div 
                class="h-full bg-accent transition-all duration-500 ease-out"
                style="width: {syncProgress}%"
              ></div>
            </div>
          </div>
        </div>
      {:else if exportSimStep === "success"}
        <div class="flex flex-col items-center text-center gap-6 py-10" in:scale>
          <div class="w-20 h-20 rounded-full bg-accent flex items-center justify-center shadow-[0_0_40px_rgba(212,163,115,0.4)]">
            <svg class="w-8 h-8 text-wave-900" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="4" stroke-linecap="round" stroke-linejoin="round">
              <polyline points="20 6 9 17 4 12" />
            </svg>
          </div>
          <div>
            <h2 class="text-2xl font-bold text-white mb-2 uppercase tracking-tight">{$t('result.modal.success')}</h2>
            <p class="text-white/40 text-sm">{$t('result.modal.success_desc', activeTracker)}</p>
          </div>
        </div>
      {/if}
    </div>
  </div>
{/if}

<style>
  .custom-scrollbar::-webkit-scrollbar {
    height: 4px;
    width: 4px;
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

  .hide-scrollbar {
    scrollbar-width: none;
    -ms-overflow-style: none;
  }
  .hide-scrollbar::-webkit-scrollbar {
    display: none;
  }

  :global(.animate-spin-slow) {
    animation: spin 3s linear infinite;
  }
</style>
