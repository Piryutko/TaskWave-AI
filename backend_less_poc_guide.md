# Guide: Implementing a Backend-less POC for AI Applications

This guide describes the approach used in the **ToneWave AI** project to demonstrate AI-like behavior (streaming responses, transformations) without a real backend or LLM API. This is ideal for early-stage POCs, UX demonstrations, and investor pitches.

## Core Concept
Instead of calling an external API, all "intelligent" behavior is simulated locally in the frontend. We use hardcoded mappings for specific "wow-factor" inputs and fallback templates for everything else.

## 1. The Simulation Engine ([simulation.ts](file:///c:/Users/Maks/Desktop/Projects/ToneWave%20AI/tonewave-web/src/lib/simulation.ts))
The logic resides in a dedicated library file. It uses TypeScript generators to simulate streaming.

### Response Mapping
Create a dictionary of expected inputs and their transformed versions.
```typescript
const RESPONSE_MAPPING = {
    "hello": {
        PROFESSIONAL: "Greetings. How may I assist you today?",
        KIND: "Hi there! I'm so happy to help you!",
        // ...
    }
};
```

### Streaming Simulation
Use `async function*` to yield text chunks with random delays.
```typescript
export async function* simulateStreamingResponse(text: string, mode: string) {
    const response = RESPONSE_MAPPING[text.toLowerCase()] || "Default fallback response...";
    
    const chunks = response.split(' ');
    for (const chunk of chunks) {
        // Yield a chunk and wait to simulate "thinking/typing"
        await new Promise(r => setTimeout(r, 30 + Math.random() * 70));
        yield chunk + ' ';
    }
}
```

## 2. Frontend Integration (SvelteKit Example)
In your component ([+page.svelte](file:///c:/Users/Maks/Desktop/Projects/ToneWave%20AI/tonewave-web/src/routes/+page.svelte)), consume the generator using a `for await...of` loop.

```typescript
let resultText = "";
let isProcessing = false;

async function handleTransform() {
    isProcessing = true;
    resultText = "";

    const stream = simulateStreamingResponse(userInput, selectedMode);
    
    for await (const chunk of stream) {
        resultText += chunk; // UI updates reactively as chunks are appended
    }
    
    isProcessing = false;
}
```

## 3. Simulating Other Operations
For things like "sending a message" or "connecting to a service", use simple `setTimeout` to simulate latency.

```typescript
async function handleSendMessage() {
    isSending = true;
    await new Promise(r => setTimeout(r, 1500)); // Simulate network lag
    isSending = false;
    isSent = true;
}
```

## Why Use This Approach?
1.  **Zero Cost**: No LLM API costs (OpenAI/Anthropic) during the POC phase.
2.  **Instant Setup**: No backend infrastructure, environment variables, or API keys needed.
3.  **Perfect UI/UX Control**: You can guarantee "perfect" responses for specific demo scenarios to show the "happy path" clearly.
4.  **Speed**: Faster to iterate on the UI when you don't have to worry about API availability or latency.

> [!TIP]
> Use this for 1.0/POC versions. Once the UI/UX is validated, replace the [simulateStreamingResponse](file:///c:/Users/Maks/Desktop/Projects/ToneWave%20AI/tonewave-web/src/lib/simulation.ts#242-265) function with a real fetch call to your LLM proxy.
