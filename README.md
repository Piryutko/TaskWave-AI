# TaskWave AI 🌊

> **Ваш AI Project Manager. Слушает встречи — создаёт документацию.**

TaskWave превращает записи митингов в готовые задачи, эпики и критерии приёмки. Автоматически, без ручной работы.

---

## Идея

Каждый митинг заканчивается одинаково: хорошие идеи обсудили, но половина потерялась, задачи не зафиксированы, документацию писать лень. Команда тратит часы на то, что должна делать машина.

**TaskWave решает это одним действием:** загружаете запись → получаете готовый документ.

---

## Как работает

```
Запись митинга
      │
      ▼
 Whisper STT          — локальное распознавание речи
      │
      ▼
 AI Корректор          — исправляет «делик» → Daily, «жира» → Jira
      │
      ▼
 LLM Аналитик         — генерирует эпики, задачи, DoD, AC
      │
      ▼
Готовый документ       — .txt / экспорт в Jira, Notion (в разработке)
```

---

## Текущий статус проекта

### ✅ Фаза 1 — AudioPM Local (готово)
CLI-движок, который работает полностью офлайн на вашем компьютере.

- Локальная расшифровка через Whisper (Base / Small / Medium)
- Двухшаговый AI-конвейер на Ollama (любая локальная LLM)
- Поддержка MP3, WAV, M4A, OGG, FLAC
- Интерактивный терминальный интерфейс
- Два режима: эпики + декомпозиция / плоский список задач
- DoD и Acceptance Criteria опционально

### 🚧 Фаза 2 — TaskWave API (в разработке)
ASP.NET Core Minimal API — HTTP-обёртка над движком с SSE-стримингом прогресса.

### 🔭 Фаза 3 — TaskWave Web (планируется)
Веб-интерфейс на Next.js + TypeScript. Загрузка файла через браузер, live progress bar, история документов.

### 🔭 Фаза 4 — TaskWave Cloud (планируется)
SaaS-платформа: командные воркспейсы, экспорт в Jira / Linear / Notion, Slack-бот, мобильное приложение.

---

## Структура репозитория

```
TaskWave-AI/
├── AudioPM.Local/        ← CLI движок (Фаза 1) — здесь
│   ├── Services/         — WhisperService, OllamaService
│   ├── Models/           — модели данных
│   ├── UI/               — TUI-интерфейс (Spectre.Console)
│   └── Program.cs        — точка входа
│
├── TaskWave.Engine/      ← class library (Фаза 2, скоро)
├── TaskWave.Api/         ← ASP.NET Core API (Фаза 2, скоро)
└── taskwave-web/         ← Next.js фронтенд (Фаза 3, скоро)
```

---

## Технологии

| Слой | Технология |
|---|---|
| Движок | .NET 8, C# 12 |
| STT | OpenAI Whisper (локально, через Whisper.net) |
| LLM | Ollama (llama3, mistral, deepseek и др.) |
| Аудио | NAudio |
| CLI | Spectre.Console |
| API | ASP.NET Core Minimal API |
| Фронтенд | Next.js + TypeScript + shadcn/ui |

---

## Быстрый старт (Фаза 1 — CLI)

```bash
git clone https://github.com/Piryutko/TaskWave-AI.git
cd TaskWave-AI/AudioPM.Local
dotnet run
```

Подробная инструкция → [`AudioPM.Local/README.md`](./AudioPM.Local/README.md)

---

## Лицензия

MIT
