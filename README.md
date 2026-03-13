# TaskWave AI 🌊

[English](#english) | [日本語](#japanese)

> **Ускорьте работу над проектом: мгновенное превращение аудио-инсайтов в управляемые задачи.**

TaskWave — это интеллектуальная экосистема, которая автоматизирует путь от голосового обсуждения до структурированного бэклога в Jira, Linear или Trello.

---

## 🚀 Видение Прототипа

TaskWave AI — это не просто инструмент для транскрибации, а **AI-Архитектор Проектов**. Мы создаем мост между живым человеческим общением и формализованными процессами управления разработкой.

### Наши цели в рамках прототипа:
- **Zero Manual Work**: Избавление менеджеров от ручного заполнения тасков после митингов.
- **Context Preservation**: Точное извлечение требований (DoD, AC) без потери технических нюансов.
- **Cross-Platform Sync**: Бесшовная синхронизация между различными трекерами задач.

---

## 🔥 Главные Преимущества

- **Локальная Приватность**: Вся обработка аудио (Whisper) происходит на вашем устройстве. Данные не покидают контур.
- **Интеллектуальная Группировка**: AI автоматически формирует Эпики (Epics) и декомпозирует их на конкретные User Stories.
- **Мультиязычный Концепт**: Поддержка RU, EN и JP на уровне интерфейса и генерации контента.
- **Премиальный UX**: Интерфейс в стиле Wave (Glassmorphism), обеспечивающий плавный и интуитивный опыт.

---

## 🛠 Функционал

1. **Audio-to-Task Engine**: Многослойный конвейер (Whisper + LLM) для извлечения сути.
2. **Scenario Manager**: Предустановленные шаблоны для дейликов, ретроспектив и планирований.
3. **Live Progress Tracking**: Визуализация каждого этапа обработки данных в реальном времени.
4. **Smart Export**: Симуляция глубокой интеграции с Jira, Linear и Trello с маппингом статусов и проектов.

---

## 🏁 Быстрый старт

Проект состоит из трех частей: CLI-движка, Backend API и SvelteKit фронтенда.

```bash
# 1. Запустить API (порт 5000)
cd TaskWave.Api
dotnet run --urls "http://localhost:5000"

# 2. Запустить фронтенд (порт 5173)
cd ../taskwave-web
npm install
npm run dev
```

- **GitHub Repository**: [https://github.com/Piryutko/TaskWave-AI](https://github.com/Piryutko/TaskWave-AI)
- **Local Demo**: [http://localhost:5173](http://localhost:5173)

---

<div id="english"></div>

# TaskWave AI (English) 🌊

> **Accelerate your project: instant transformation of audio insights into manageable tasks.**

TaskWave is an intelligent ecosystem that automates the journey from voice discussions to a structured backlog in Jira, Linear, or Trello.

---

## 🚀 Prototype Vision

TaskWave AI is not just a transcription tool, but an **AI Project Architect**. We create a bridge between live human communication and formalized development management processes.

### Our goals within the prototype:
- **Zero Manual Work**: Freeing managers from manual task entry after meetings.
- **Context Preservation**: Accurate extraction of requirements (DoD, AC) without losing technical nuances.
- **Cross-Platform Sync**: Seamless synchronization across various task trackers.

---

## 🔥 Key Advantages

- **Local Privacy**: All audio processing (Whisper) happens on your device. Data stays within your perimeter.
- **Intelligent Grouping**: AI automatically creates Epics and decomposes them into specific User Stories.
- **Multilingual Concept**: Support for RU, EN, and JP at both the UI and content generation levels.
- **Premium UX**: Wave-style (Glassmorphism) interface providing a smooth and intuitive experience.

---

<div id="japanese"></div>

# TaskWave AI (Japanese) 🌊

> **プロジェクトを加速：音声のインサイトを瞬時に管理可能なタスクへ変換します。**

TaskWaveは、音声での議論からJira、Linear、Trelloの構造化されたバックログへの道のりを自動化するインテリジェントなエコシステムです。

---

## 🚀 プロトタイプのビジョン

TaskWave AIは単なる文字起こしツールではなく、**AIプロジェクトアーキテクト**です。人間同士のライブなコミュニケーションと、形式化された開発管理プロセスの間に架け橋を築きます。

### プロトタイプにおける目標：
- **Zero Manual Work**: 会議後の手動でのタスク入力をマネージャーから解放します。
- **Context Preservation**: 技術的なニュアンスを失うことなく、要件（DoD、AC）を正確に抽出します。
- **Cross-Platform Sync**: 様々なタスク追跡ツール間でのシームレスな同期を実現します。

---

## 🔥 主な利点

- **ローカルプライバシー**: すべての音声処理（Whisper）はデバイス上で行われます。データが外部に漏れることはありません。
- **インテリジェントなグループ化**: AIが自動的にエピックを作成し、それらを具体的なユーザーストーリーに分解します。
- **多言語コンセプト**: UIとコンテンツ生成の両方のレベルで、RU、EN、JPをサポートしています。
- **プレミアムUX**: Waveスタイル（グラスモーフィズム）のインターフェースにより、スムーズで直感的な体験を提供します。

---

## Лицензия / License

MIT
