<script lang="ts">
  import "../app.css";
  import { page } from "$app/stores";

  const navLinks = [
    { href: "/", label: "Главная" },
    { href: "/upload", label: "Попробовать" },
  ];
</script>

<div class="min-h-screen flex flex-col" style="background: #0a0a0f;">
  <!-- Nav -->
  <header class="sticky top-0 z-50 glass border-b border-white/5">
    <div class="max-w-6xl mx-auto px-6 h-16 flex items-center justify-between">
      <a href="/" class="flex items-center gap-2.5 no-underline">
        <div
          class="w-8 h-8 rounded-lg flex items-center justify-center"
          style="background: linear-gradient(135deg,#7c3aed,#22d3ee);"
        >
          <svg
            width="18"
            height="18"
            viewBox="0 0 24 24"
            fill="none"
            stroke="white"
            stroke-width="2.5"
          >
            <path
              d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z"
            />
            <path d="M8 12h8M12 8v8" />
          </svg>
        </div>
        <span class="font-bold text-lg text-white tracking-tight">TaskWave</span
        >
        <span class="badge text-xs">Alpha</span>
      </a>

      <nav class="flex items-center gap-1">
        {#each navLinks as link}
          <a
            href={link.href}
            class="px-4 py-2 rounded-lg text-sm font-medium transition-all duration-200 no-underline
                    {$page.url.pathname === link.href
              ? 'bg-white/10 text-white'
              : 'text-slate-400 hover:text-white hover:bg-white/5'}"
          >
            {link.label}
          </a>
        {/each}
        <a href="/upload" class="btn-primary ml-3 text-sm py-2 px-4">
          Загрузить аудио →
        </a>
      </nav>
    </div>
  </header>

  <!-- Page content -->
  <main class="flex-1">
    <slot />
  </main>

  <!-- Footer -->
  <footer class="border-t border-white/5 py-8 mt-20">
    <div
      class="max-w-6xl mx-auto px-6 flex items-center justify-between text-sm text-slate-500"
    >
      <span>© 2026 TaskWave AI. Все права защищены.</span>
    </div>
  </footer>
</div>
