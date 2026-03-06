# TaskWave AI 🌊

> **Ваш AI Project Manager. Слушает встречи — создаёт документацию.**

TaskWave превращает записи митингов в готовые задачи, эпики и критерии приёмки. Автоматически, без ручной работы.

---

## Как работает

```
Запись митинга
      │
      ▼
 AI Распознавание    — локальная расшифровка речи
      │
      ▼
 AI Корректор        — исправляет технический жаргон
      │
      ▼
 LLM Аналитик        — генерирует эпики, задачи, DoD, AC
      │
      ▼
Готовый документ     — .txt / экспорт в Jira, Notion (в разработке)
```

---

## Текущий статус

### ✅ Фаза 1 — AudioPM Local (готово)
CLI-движок, который работает полностью офлайн.

- Локальная расшифровка (Base / Small / Medium)
- Двухшаговый AI-конвейер на Ollama (любая локальная LLM)
- Поддержка MP3, WAV, M4A, OGG, FLAC
- Интерактивный терминальный интерфейс
- Два режима: эпики + декомпозиция / плоский список задач
- DoD и Acceptance Criteria опционально

### ✅ Фаза 2 — TaskWave API (готово)
ASP.NET Core Minimal API над движком.

- `POST /api/jobs` — загрузка аудио, создание задания
- `GET /api/jobs/{id}/stream` — SSE стриминг прогресса в реальном времени
- `GET /api/jobs/{id}/result` — готовый документ
- `GET /api/health` — статус + список доступных AI-моделей
- Авто-определение Whisper и Ollama моделей при запуске
- CORS для фронтенда

### ✅ Фаза 3 — TaskWave Web (готово)
SvelteKit фронтенд.

- Главная страница с описанием продукта
- Drag-and-drop загрузка аудио
- Выбор режима (Эпики / Задачи) и DoD/AC опция
- Live прогресс обработки по 4 шагам через SSE
- Страница результата с копированием и скачиванием .txt

---

## Быстрый старт

```bash
# 1. Клонировать
git clone https://github.com/Piryutko/TaskWave-AI.git
cd TaskWave-AI

# 2. Запустить API (порт 5000)
cd TaskWave.Api
dotnet run --urls "http://localhost:5000"

# 3. Запустить фронтенд (порт 5173)
cd ../taskwave-web
npm install
npm run dev

# 4. Открыть браузер
# http://localhost:5173
```

> **Требования:** .NET 8 SDK, Node.js 18+, [Ollama](https://ollama.ai/) с любой моделью (`ollama pull llama3.2`)

---

## Структура репозитория

```
TaskWave-AI/
├── AudioPM.Local/        ← CLI движок (Фаза 1)
│   ├── Services/         — WhisperService, OllamaService
│   ├── Models/           — модели данных
│   ├── UI/               — TUI интерфейс (Spectre.Console)
│   └── Program.cs
│
├── TaskWave.Api/         ← ASP.NET Core API (Фаза 2)
│   ├── Jobs/             — JobState, JobManager
│   ├── Services/         — WhisperService, OllamaService
│   ├── Models/           — AppModels
│   └── Program.cs        — эндпоинты + DI + CORS
│
└── taskwave-web/         ← SvelteKit фронтенд (Фаза 3)
    └── src/routes/
        ├── +page.svelte          — главная
        ├── upload/               — загрузка аудио
        ├── processing/[jobId]/   — live прогресс
        └── result/[jobId]/       — готовый документ
```

---

## Технологии

| Слой | Технология |
|---|---|
| CLI движок | .NET 8, C# 12 |
| AI Распознавание | Whisper.net (локально) |
| Аудио конвертация | NAudio (MediaFoundation) |
| LLM | Ollama HTTP API |
| HTTP API | ASP.NET Core Minimal API |
| Стриминг | Server-Sent Events (SSE) |
| Фронтенд | SvelteKit + TypeScript |
| Стили | Tailwind CSS v4 |

---

## 📋 Бэклог

### 🔐 Аутентификация и пользователи
- [ ] Регистрация / вход (Supabase Auth или Clerk)
- [ ] Личный кабинет пользователя
- [ ] Командные воркспейсы и роли

### 💾 История и хранение
- [ ] Сохранять результаты в БД (SQLite → PostgreSQL)
- [ ] Страница истории всех обработанных митингов
- [ ] Поиск по документам
- [ ] Повторная генерация без перезагрузки аудио

### 📤 Интеграции и экспорт
- [ ] Экспорт в Jira (создание задач через Jira API)
- [ ] Экспорт в Notion
- [ ] Экспорт в Linear
- [ ] Скачивание в форматах Markdown и PDF

### 🧠 Улучшение качества AI
- [ ] Выбор AI-модели прямо из фронтенда
- [ ] Поддержка нескольких языков (EN, DE, UK)
- [ ] Кастомный системный промпт для команды
- [ ] Режимы «ретроспектива», «1:1», «планирование спринта»

### 🚀 Деплой и инфраструктура
- [ ] Docker-контейнер для TaskWave API
- [ ] `docker-compose.yml` для полного стека (API + Ollama)
- [ ] CI/CD pipeline на GitHub Actions
- [ ] Деплой API на Railway / Fly.io
- [ ] Деплой фронтенда на Cloudflare Pages / Vercel

### 💰 SaaS монетизация
- [ ] Тарифные планы (Free / Pro / Team)
- [ ] Оплата через Stripe
- [ ] Лимиты по минутам аудио в месяц

---

## Лицензия

MIT
