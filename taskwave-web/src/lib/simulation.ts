/**
 * Движок симуляции для демонстрации AI-функционала без бэкенда.
 * Использует генераторы для имитации потоковой передачи (streaming).
 */

const RESPONSE_MAPPING: Record<string, string> = {
  "daily": "═══ DAILY STANDUP SUMMARY ═══\n\n📋 Task 1: Update API documentation (In Progress)\n📋 Task 2: Fix login page glassmorphism (Done)\n📋 Task 3: Implement simulation engine (New)\n\nDoD: All tests passed on local environment.",
  "sprint": "═══ SPRINT PLANNING: WAVE PROJECT ═══\n\n🚀 EPIC 1: Design System Core\n  - Task 1.1: Define Tailwind tokens\n  - Task 1.2: Implement glass containers\n\n🚀 EPIC 2: Simulation Layer\n  - Task 2.1: Generator-based streaming\n  - Task 2.2: latency simulation\n\nAC: Smooth 60fps animations during streaming.",
  "meeting": "═══ MEETING NOTES: ARCHITECTURE REVIEW ═══\n\n💡 Decision: Use SvelteKit for frontend.\n💡 Decision: Implement backend-less POC for initial demo.\n\n✅ Action Item: Update design system guide.\n✅ Action Item: Create simulation utility.",
  "report": "═══ PROJECT STATUS REPORT ═══\n\n📈 Progress: 85%\n🟢 Health: Excellent\n\nKey Highlights:\n- Premium Wave aesthetic implemented.\n- Zero-cost AI simulation active.\n- Enhanced UX with micro-animations."
};

const FALLBACK_RESPONSE = "═══ TASKWAVE AI PROCESSING ═══\n\nI've analyzed your input and started crumbling it down into actionable tasks.\n\n📋 General Task: Process data structure\n📋 General Task: Apply design tokens\n\nDoD: Integration complete.";

/**
 * Симулирует задержку сети
 */
export async function simulateDelay(ms: number = 1000) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

/**
 * Симулирует потоковый ответ (streaming)
 */
export async function* simulateStreamingResponse(text: string) {
  const lowercaseInput = text.toLowerCase();
  let response = FALLBACK_RESPONSE;

  // Ищем совпадение в маппинге
  for (const key in RESPONSE_MAPPING) {
    if (lowercaseInput.includes(key)) {
      response = RESPONSE_MAPPING[key];
      break;
    }
  }

  const chunks = response.split(' ');
  for (const chunk of chunks) {
    // Имитируем "раздумья" AI (случайная задержка между словами)
    await new Promise(r => setTimeout(r, 40 + Math.random() * 80));
    yield chunk + ' ';
  }
}
