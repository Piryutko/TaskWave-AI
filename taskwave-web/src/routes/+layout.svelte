<script lang="ts">
  import "../app.css";
</script>

<div class="min-h-screen relative overflow-hidden bg-wave-900">
  <!-- Atmospheric Glows -->
  <div class="fixed inset-0 pointer-events-none z-0 overflow-hidden">
    <div class="absolute top-[-10%] left-[-10%] w-[60%] h-[60%] bg-accent/7 rounded-full blur-[120px] animate-pulse-glow animate-bg-flow"></div>
    <div class="absolute bottom-[-10%] right-[-10%] w-[50%] h-[50%] bg-accent/3 rounded-full blur-[120px] animate-pulse-glow animate-bg-flow" style="animation-delay: -5s; animation-duration: 6s, 20s;"></div>
    <div class="absolute top-[20%] right-[10%] w-[30%] h-[30%] bg-accent/3 rounded-full blur-[80px] animate-pulse-glow animate-bg-flow" style="animation-delay: -2s; animation-duration: 5s, 18s;"></div>
  </div>

  <!-- Page content -->
  <main class="relative z-10">
    <slot />
  </main>
</div>
