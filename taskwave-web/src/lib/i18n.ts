import { writable, derived } from 'svelte/store';

export const lang = writable('ru');

export const translations: Record<string, any> = {
  ru: {
    common: {
      strategy: 'Стратегия развития',
      ai_generator: 'AI-генератор задач',
      conceptual_demo: 'Conceptual Demo',
      back: 'Назад',
      close_modal: 'Закрыть',
      sync: 'Синхронизация',
      strategic: 'Стратегическая',
      research: 'Исследование',
      review: 'Обзор',
      design_review_type: 'Дизайн-ревью',
      management: 'Управление',
      '15min': '15 мин',
      '45min': '45 мин',
      '30min': '30 мин',
      '25min': '25 мин',
      '40min': '40 мин',
      '60min': '60 мин',
      today_1030: 'Сегодня, 10:30',
      yesterday_1415: 'Вчера, 14:15',
      march_12: '12 Марта',
      march_10: '10 Марта',
      today_0900: 'Сегодня, 09:00',
      yesterday_1630: 'Вчера, 16:30',
      main: 'Основные',
      m: 'м',
      s: 'с',
      attendees: 'Участники',
    },
    meetings: {
      daily: { title: 'Дейлик команды TimeWave', desc: 'Разбор текущих блокировок и инсайдов. Формирование оперативного бэклога.' },
      stakeholder: { title: 'План со стейкхолдером', desc: 'Согласование требований безопасности и масштабирования системы.' },
      analyst: { title: 'Сессия аналитика', desc: 'Детализация логики тарифов и интеграция с финансовым движком.' },
      review: { title: 'Обзор функционала v2.0', desc: 'Демонстрация новых возможностей и фиксация фидбека по UI/UX.' },
      design: { title: 'Встреча с дизайнером', desc: 'Обсуждение прототипа новой дизайн-системы и финализация UI-кита.' },
      pmo: { title: 'Встреча с проектным офисом', desc: 'Синхронизация дорожной карты проектов и распределение ресурсов на Q2.' }
    },
    landing: {
      hero_title: 'TaskWave AI',
      hero_subtitle: 'Ускорьте работу над проектом: мгновенное превращение аудио-инсайтов в управляемые задачи.',
      cta_button: 'Загрузить запись встречи',
      roadmap_title: 'Этапы разработки',
      roadmap_vision: 'Roadmap & Market Vision',
      market_title: 'Прогноз и Рынок',
      contact: 'Связь с основателем',
      roadmap: [
        { title: 'AI-генератор задач', status: 'Завершено', desc: 'Завершена база симуляции и визуальный стиль Wave.' },
        { title: 'MVP & Beta', status: 'В процессе', desc: 'Интеграция с локальными LLM (Ollama) и Whisper.' },
        { title: 'Scaling', status: 'Ожидание', desc: 'Облачная инфраструктура и Enterprise-интеграции.' }
      ],
      market_data: [
        { title: 'AI Productivity Hub', desc: 'Растущий спрос на инструменты автоматизации встреч и формализации знаний.' },
        { title: 'Integration Ecosystem', desc: 'Потенциал встраивания в экосистемы Jira, Slack и Teams.' }
      ]
    },
    upload: {
      library_title: 'Менеджер сценариев',
      selected_file: 'Выбранный файл',
      start_btn: 'Начать анализ встречи',
      placeholder: 'Выберите сценарий из библиотеки слева',
      duration: 'Длительность',
      size: 'Размер',
      attendees: 'уч.',
      open_library: 'ОТКРЫТЬ БИБЛИОТЕКУ',
      config_title: 'Настройка анализа',
      step_1: '01. Выбор аудиофайла',
      step_2: '02. Настройка анализа',
      step_3: '03. Запуск',
      mode_label: 'Режим',
      details_label: 'Детализация задач',
      details_desc: 'Критерии готовности и приёмки',
      ready_to_analyze: 'Готов к анализу',
      analyze_desc: (mode: string, details: boolean) => `Будут извлечены задачи в режиме «${mode}» ${details ? 'с детальными критериями готовности и приёмки' : ''}.`,
      analyzing: 'АНАЛИЗ...',
      explorer: {
        title: 'Записи',
        recent: 'Недавние',
        favorites: 'Избранное',
        projects: 'Проекты'
      }
    },
    processing: {
      title: 'Анализ аудиопотока',
      subtitle: 'Нейросеть TaskWave сегментирует данные и формирует структуру задач...',
      main_progress: 'Общий прогресс',
      status_ready: 'Готово',
      status_processing: 'В процессе',
      time: 'Время',
      progress_label: 'Прогресс',
      success: 'Анализ завершен успешно',
      steps: [
        { label: 'Инициализация AI', sublabel: 'Загрузка параметров проекта...' },
        { label: 'Распознавание', sublabel: 'Преобразование аудио в текст...' },
        { label: 'Корректор', sublabel: 'Исправление тех. жаргона...' },
        { label: 'Генерация задач', sublabel: 'Формирование структуры...' }
      ]
    },
    result: {
      title: 'Архитектура задач',
      session_name: 'Анализ сессии',
      sync_jira: 'Синхронизация с Jira',
      sync_linear: 'Синхронизация с Linear',
      sync_trello: 'Синхронизация с Trello',
      export_btn: 'Экспорт в Jira',
      download_btn: 'Скачать .txt',
      hierarchy: 'Иерархия',
      list: 'Список',
      epics: 'Эпики',
      formalizing: 'Формализация задач...',
      analysis_ready: 'Анализ готов',
      epic_label: 'Эпик',
      ac_label: 'Критерии приёмки',
      dod_label: 'Критерии готовности',
      details_btn: 'ДЕТАЛИ',
      hide_btn: 'СКРЫТЬ',
      active_badge: 'Активна',
      desc_label: 'Описание задачи',
      source_label: 'Источник анализа',
      export_trackers_label: 'Экспорт в трекеры',
      new_tasks_link: 'К новым задачам →',
      export_success_msg: 'Задачи отправлены в',
      modal: {
        title: 'Экспорт в',
        subtitle: 'Настройте параметры интеграции для синхронизации бэклога.',
        target_project: 'Выбор проекта',
        target_status: 'Целевой статус',
        confirm: 'ПОДТВЕРДИТЬ И СИНХРОНИЗИРОВАТЬ',
        success: 'УСПЕШНО!',
        success_desc: (tracker: string) => `Задачи успешно добавлены в ${tracker}.`,
        sync_steps: [
          (tracker: string) => `Авторизация в ${tracker}...`,
          () => 'Проверка прав доступа к проекту...',
          (count: number) => `Синхронизация ${count} задач...`,
          () => 'Создание связей между эпиками...',
          () => 'Готово!'
        ]
      }
    },
    projects: {
      'daily-standup': [
        {
          title: 'Продуктовые фичи (Sprint 12)',
          desc: 'Фокус на критических обновлениях пользовательского интерфейса и интеграции ключевых API профиля для текущего спринта.',
          tasks: [
            {
              title: 'Реализация ленты уведомлений',
              desc: 'Создание системы push-уведомлений и встроенного колокольчика для событий в реальном времени.',
              dod: ['WebSocket соединение стабильно', 'Уведомления сохраняются в БД'],
              ac: ['Отображение на мобилках', 'Push-токеры валидны']
            },
            {
              title: 'Интеграция API профиля',
              desc: 'Связка фронтенда с новыми эндпоинтами редактирования данных пользователя.',
              dod: ['Аватары загружаются в S3', 'Валидация полей на бэкенде'],
              ac: ['Обработка ошибок 400/500']
            }
          ]
        },
        {
          title: 'Стабильность и Качество',
          desc: 'Обеспечение отказоустойчивости системы через глубокое тестирование и устранение проблем с утечками ресурсов.',
          tasks: [
            {
              title: 'Покрытие тестами модуля Auth',
              desc: 'Написание юнит и интеграционных тестов для логики авторизации и восстановления пароля.',
              dod: ['Покрытие по строкам > 80%', 'CI пайплайн проходит'],
              ac: ['Тестирование edge-cases']
            },
            {
              title: 'Исправление утечки в WebSocket',
              desc: 'Анализ heap dump и устранение проблемы с накоплением слушателей при реконнекте.',
              dod: ['Память стабильна после 1000 коннектов', 'Логи очищены'],
              ac: ['Авто-реконнект < 2 сек']
            }
          ]
        }
      ],
      'stakeholder-brief': [
        {
          title: 'Стратегическое планирование',
          desc: 'Долгосрочное видение развития продукта, включая масштабирование команды и приоритизацию ключевых рыночных инициатив.',
          tasks: [
            {
              title: 'Оптимизация ресурсной матрицы Q3',
              desc: 'Перераспределение ключевых разработчиков между стримами для ускорения Time-to-Market.',
              dod: ['Ресурсный план утвержден', 'Риски митигированы'],
              ac: ['Балансировка нагрузки подтверждена']
            }
          ]
        }
      ],
      'design-review': [
        {
          title: 'Wave Design System v4.0',
          desc: 'Эволюция визуального языка Wave, централизация токенов и создание набора высококачественных микро-взаимодействий.',
          tasks: [
            {
              title: 'Система дизайн-токенов',
              desc: 'Разработка библиотеки переменных для управления цветами, отступами и типографикой.',
              dod: ['JSON экспорт для веба/мобайла', 'Синхронизация с Figma'],
              ac: ['Поддержка темной темы']
            }
          ]
        }
      ],
      'retrospective': [
        {
          title: 'Эффективность процессов',
          desc: 'Автоматизация рутинных задач и ускорение цикла Code Review для повышения скорости разработки (Velocity).',
          tasks: [
            {
              title: 'Авто-пайплайн Code Review',
              desc: 'Интеграция ботов для проверки стиля кода и покрытия тестами в PR.',
              dod: ['Блокировка пуша при ошибках', 'Статистика по ревьюерам'],
              ac: ['Сокращение времени на 30%']
            }
          ]
        }
      ]
    }
  },
  en: {
    common: {
      strategy: 'Strategy',
      ai_generator: 'AI Task Generator',
      conceptual_demo: 'Conceptual Demo',
      back: 'Back',
      close_modal: 'Close',
      sync: 'Sync',
      strategic: 'Strategic',
      research: 'Research',
      review: 'Review',
      design_review_type: 'Design Review',
      management: 'Management',
      '15min': '15 min',
      '45min': '45 min',
      '30min': '30 min',
      '25min': '25 min',
      '40min': '40 min',
      '60min': '60 min',
      today_1030: 'Today, 10:30',
      yesterday_1415: 'Yesterday, 14:15',
      march_12: 'March 12',
      march_10: 'March 10',
      today_0900: 'Today, 09:00',
      yesterday_1630: 'Yesterday, 16:30',
      main: 'Main',
      m: 'm',
      s: 's',
      attendees: 'Attendees',
    },
    meetings: {
      daily: { title: 'TimeWave Team Daily', desc: 'Review current blockers and insights. Forming the operational backlog.' },
      stakeholder: { title: 'Stakeholder Planning', desc: 'Sync on security requirements and system scaling.' },
      analyst: { title: 'Analyst Session', desc: 'Detallization of tariff logic and integration with the financial engine.' },
      review: { title: 'v2.0 Feature Review', desc: 'Demo of new capabilities and capturing UI/UX feedback.' },
      design: { title: 'Designer Sync', desc: 'Discussion of the new design system prototype and UI kit finalization.' },
      pmo: { title: 'PMO Meeting', desc: 'Syncing project roadmap and resource allocation for Q2.' }
    },
    landing: {
      hero_title: 'TaskWave AI',
      hero_subtitle: 'Accelerate your project: instant transformation of audio insights into manageable tasks.',
      cta_button: 'Upload Meeting Recording',
      roadmap_title: 'Development Stages',
      roadmap_vision: 'Roadmap & Market Vision',
      market_title: 'Forecast & Market',
      contact: 'Contact Founder',
      roadmap: [
        { title: 'AI Task Generator', status: 'Completed', desc: 'Simulation base and Wave visual style complete.' },
        { title: 'MVP & Beta', status: 'In Progress', desc: 'Integration with local LLMs (Ollama) and Whisper.' },
        { title: 'Scaling', status: 'Pending', desc: 'Cloud infrastructure and Enterprise integrations.' }
      ],
      market_data: [
        { title: 'AI Productivity Hub', desc: 'Growing demand for meeting automation and knowledge formalization.' },
        { title: 'Integration Ecosystem', desc: 'Potential for embedding into Jira, Slack, and Teams.' }
      ]
    },
    upload: {
      library_title: 'Scenario Manager',
      selected_file: 'Selected File',
      start_btn: 'Start Meeting Analysis',
      placeholder: 'Select a scenario from the library on the left',
      duration: 'Duration',
      size: 'Size',
      attendees: 'att.',
      open_library: 'OPEN LIBRARY',
      config_title: 'Generation Settings',
      step_1: '01. Audio Selection',
      step_2: '02. Analysis Settings',
      step_3: '03. Launch',
      mode_label: 'Mode',
      details_label: 'Task Details',
      details_desc: 'Definition of Done & Acceptance Criteria',
      ready_to_analyze: 'Ready for Analysis',
      analyze_desc: (mode: string, details: boolean) => `Tasks will be extracted in "${mode}" mode ${details ? 'with detailed criteria' : ''}.`,
      analyzing: 'ANALYZING...',
      explorer: {
        title: 'Recordings',
        recent: 'Recent',
        favorites: 'Favorites',
        projects: 'Projects'
      }
    },
    processing: {
      title: 'Audio Stream Analysis',
      subtitle: 'TaskWave AI is segmenting data and structuring tasks...',
      main_progress: 'Total Progress',
      status_ready: 'Ready',
      status_processing: 'Processing',
      time: 'Time',
      progress_label: 'Progress',
      success: 'Analysis completed successfully',
      steps: [
        { label: 'AI Initialization', sublabel: 'Loading project parameters...' },
        { label: 'Transcription', sublabel: 'Converting audio to text...' },
        { label: 'Correction', sublabel: 'Fixing technical jargon...' },
        { label: 'Task Generation', sublabel: 'Structuring output...' }
      ]
    },
    result: {
      title: 'Task Architecture',
      session_name: 'Session Analysis',
      sync_jira: 'Sync with Jira',
      sync_linear: 'Sync with Linear',
      sync_trello: 'Sync with Trello',
      export_btn: 'Export to Jira',
      download_btn: 'Download .txt',
      hierarchy: 'Hierarchy',
      list: 'List',
      epics: 'Epics',
      formalizing: 'Formalizing tasks...',
      analysis_ready: 'Analysis ready',
      epic_label: 'Epic',
      ac_label: 'Acceptance Criteria',
      dod_label: 'Definition of Done',
      details_btn: 'DETAILS',
      hide_btn: 'HIDE',
      active_badge: 'Active',
      desc_label: 'Task Description',
      source_label: 'Analysis Source',
      export_trackers_label: 'Export to trackers',
      new_tasks_link: 'To new tasks →',
      export_success_msg: 'Tasks sent to',
      modal: {
        title: 'Export to',
        subtitle: 'Configure integration parameters for backlog synchronization.',
        target_project: 'Select Project',
        target_status: 'Target Status',
        confirm: 'CONFIRM & SYNC',
        success: 'SUCCESS!',
        success_desc: (tracker: string) => `Tasks successfully added to ${tracker}.`,
        sync_steps: [
          (tracker: string) => `Authorizing in ${tracker}...`,
          () => 'Verifying project access...',
          (count: number) => `Syncing ${count} tasks...`,
          () => 'Creating epic mappings...',
          () => 'Done!'
        ]
      }
    },
    projects: {
      'daily-standup': [
        {
          title: 'Product Features (Sprint 12)',
          desc: 'Focus on critical UI updates and core profile API integration for the current sprint.',
          tasks: [
            {
              title: 'Notification Feed Implementation',
              desc: 'Create push notification system and in-app bell for real-time events.',
              dod: ['WebSocket connection stable', 'Notifications saved in DB'],
              ac: ['Mobile display works', 'Push tokens valid']
            },
            {
              title: 'Profile API Integration',
              desc: 'Linking frontend with new user data editing endpoints.',
              dod: ['Avatars upload to S3', 'Backend validation complete'],
              ac: ['Handle 400/500 errors']
            }
          ]
        },
        {
          title: 'Stability & Quality',
          desc: 'Ensure system resilience through deep testing and resolving resource leak issues.',
          tasks: [
            {
              title: 'Auth Module Test Coverage',
              desc: 'Writing unit and integration tests for auth logic and password recovery.',
              dod: ['Coverage > 80%', 'CI pipeline passes'],
              ac: ['Edge-case testing complete']
            }
          ]
        }
      ],
      'stakeholder-brief': [
        {
          title: 'Strategic Planning',
          desc: 'Long-term vision including team scaling and market initiatives prioritization.',
          tasks: [
            {
              title: 'Q3 Resource Matrix Optimization',
              desc: 'Redistribute key devs between streams to accelerate Time-to-Market.',
              dod: ['Resource plan approved', 'Risks mitigated'],
              ac: ['Load balancing verified']
            }
          ]
        }
      ],
      'design-review': [
        {
          title: 'Wave Design System v4.0',
          desc: 'Visual language evolution, token centralization and premium micro-interactions.',
          tasks: [
            {
              title: 'Design Token System',
              desc: 'Develop variable library for colors, spacing, and typography.',
              dod: ['JSON export for web/mobile', 'Figma sync'],
              ac: ['Dark mode support']
            }
          ]
        }
      ],
      'retrospective': [
        {
          title: 'Process Efficiency',
          desc: 'Routine task automation and Code Review cycle acceleration to boost velocity.',
          tasks: [
            {
              title: 'Code Review Auto-Pipeline',
              desc: 'Bot integration for code style and test coverage checks in PRs.',
              dod: ['Block push on errors', 'Reviewer stats active'],
              ac: ['Reduce time by 30%']
            }
          ]
        }
      ]
    }
  },
  jp: {
    common: {
      strategy: '開発戦略',
      ai_generator: 'AIタスクジェネレーター',
      conceptual_demo: 'Conceptual Demo',
      back: '戻る',
      close_modal: '閉じる',
      sync: '同期',
      strategic: '戦略的',
      research: '研究',
      review: 'レビュー',
      design_review_type: 'デザインレビュー',
      management: '管理',
      '15min': '15 分',
      '45min': '45 分',
      '30min': '30 分',
      '25min': '25 分',
      '40min': '40 分',
      '60min': '60 分',
      today_1030: '今日, 10:30',
      yesterday_1415: '昨日, 14:15',
      march_12: '3月 12日',
      march_10: '3月 10日',
      today_0900: '今日, 09:00',
      yesterday_1630: '昨日, 16:30',
      main: 'メイン',
      m: '分',
      s: '秒',
      attendees: '参加者',
    },
    meetings: {
      daily: { title: 'TimeWaveチームデイリー', desc: '現在のブロッカーとインサイトを確認します。運用バックログの形成。' },
      stakeholder: { title: 'ステークホルダープランニング', desc: 'セキュリティ要件とシステムスケーリングの同期。' },
      analyst: { title: 'アナリストセッション', desc: '関税ロジックの詳細化と金融エンジンとの統合。' },
      review: { title: 'v2.0 機能レビュー', desc: '新機能のデモとUI/UXフィードバックの取得。' },
      design: { title: 'デザイナー同期', desc: '新しいデザインシステムのプロトタイプとUIキットの完成についての議論。' },
      pmo: { title: 'PMO会議', desc: 'Q2のプロジェクトロードマップとリソース割り当ての同期。' }
    },
    landing: {
      hero_title: 'TaskWave AI',
      hero_subtitle: 'プロジェクトを加速：音声のインサイトを瞬時に管理可能なタスクへ変換します。',
      cta_button: '会議の録音をアップロード',
      roadmap_title: '開発フェーズ',
      roadmap_vision: 'ロードマップと市場展望',
      market_title: '予測と市場',
      contact: '創業者に連絡',
      roadmap: [
        { title: 'AIタスクジェネレーター', status: '完了', desc: 'シミュレーションベースとWaveビジュアルスタイルの完成。' },
        { title: 'MVP & ベータ', status: '進行中', desc: 'ローカルLLM（Ollama）とWhisperとの統合。' },
        { title: 'スケーリング', status: '待機中', desc: 'クラウドインフラストラクチャとエンタープライズ統合。' }
      ],
      market_data: [
        { title: 'AI生産性ハブ', desc: '会議の自動化と知識の形式化への需要の高まり。' },
        { title: '統合エコシステム', desc: 'Jira、Slack、Teamsへの組み込みの可能性。' }
      ]
    },
    upload: {
      library_title: 'シナリオマネージャー',
      selected_file: '選択されたファイル',
      start_btn: '会議の分析を開始',
      placeholder: '左のライブラリからシナリオを選択してください',
      duration: '期間',
      size: 'サイズ',
      attendees: '人',
      open_library: 'ライブラリを開く',
      config_title: '分析の設定',
      step_1: '01. 音声の選択',
      step_2: '02. 分析の設定',
      step_3: '03. 起動',
      mode_label: 'モード',
      details_label: 'タスクの詳細',
      details_desc: '完了定義と受け入れ基準',
      ready_to_analyze: '分析の準備完了',
      analyze_desc: (mode: string, details: boolean) => `タスクは「${mode}」モードで抽出されます ${details ? '（詳細な基準を含む）' : ''}。`,
      analyzing: '分析中...',
      explorer: {
        title: '録音',
        recent: '最近',
        favorites: 'お気に入り',
        projects: 'プロジェクト'
      }
    },
    processing: {
      title: '音声ストリームの分析',
      subtitle: 'TaskWave AIがデータをセグメント化し、タスクを構造化しています...',
      main_progress: '全体進捗',
      status_ready: '完了',
      status_processing: '処理中',
      time: '時間',
      progress_label: '進捗',
      success: '分析が正常に完了しました',
      steps: [
        { label: 'AI初期化', sublabel: 'プロジェクトパラメータの読み込み中...' },
        { label: '文字起こし', sublabel: '音声をテキストに変換中...' },
        { label: '校正', sublabel: '技術用語の修正中...' },
        { label: 'タスク生成', sublabel: '構造の形成中...' }
      ]
    },
    result: {
      title: 'タスクアーキテクチャ',
      session_name: 'セッション分析',
      sync_jira: 'Jiraと同期',
      sync_linear: 'Linearと同期',
      sync_trello: 'Trelloと同期',
      export_btn: 'Jiraにエクスポート',
      download_btn: '.txtをダウンロード',
      hierarchy: '階層',
      list: 'リスト',
      epics: 'エピック',
      formalizing: 'タスクの形式化中...',
      analysis_ready: '分析完了',
      epic_label: 'エピック',
      ac_label: '受け入れ基準',
      dod_label: '完了定義',
      details_btn: '詳細',
      hide_btn: '隠す',
      active_badge: 'アクティブ',
      desc_label: 'タスクの説明',
      source_label: '分析ソース',
      export_trackers_label: 'トラッカーへのエクスポート',
      new_tasks_link: '新しいタスクへ →',
      export_success_msg: 'タスクを送信しました：',
      modal: {
        title: 'へのエクスポート',
        subtitle: 'バックログ同期のための統合パラメータを設定します。',
        target_project: 'プロジェクト選択',
        target_status: 'ターゲットステータス',
        confirm: '確認して同期',
        success: '成功！',
        success_desc: (tracker: string) => `タスクが正常に ${tracker} に追加されました。`,
        sync_steps: [
          (tracker: string) => `${tracker} で認可中...`,
          () => 'プロジェクトアクセスを確認中...',
          (count: number) => `${count} 個のタスクを同期中...`,
          () => 'エピックマッピングを作成中...',
          () => '完了！'
        ]
      }
    },
    projects: {
      'daily-standup': [
        {
          title: 'プロダクト機能 (スプリント 12)',
          desc: 'UIの重要な更新と、現在のスプリントのコアプリファイルAPI統合に焦点を当てます。',
          tasks: [
            {
              title: '通知フィードの実装',
              desc: 'リアルタイムイベント用のプッシュ通知システムとアプリ内ベルを作成します。',
              dod: ['WebSocket接続が安定している', '通知がDBに保存される'],
              ac: ['モバイル表示が機能する', 'プッシュトークンが有効']
            },
            {
              title: 'プロファイルAPIの統合',
              desc: 'フロントエンドを新しいユーザーデータ編集エンドポイントにリンクします。',
              dod: ['アバターがS3にアップロードされる', 'バグエンドのバリデーション完了'],
              ac: ['400/500エラーの処理']
            }
          ]
        },
        {
          title: '安定性と品質',
          desc: '詳細なテストとリソースリーク問題の解決を通じて、システムの回復力を確保します。',
          tasks: [
            {
              title: '認証モジュールのテストカバレッジ',
              desc: '認証ロジックとパスワード回復のユニットテストおよび統合テストを作成します。',
              dod: ['カバレッジ 80%以上', 'CIパイプラインが合格'],
              ac: ['エッジケースのテスト完了']
            }
          ]
        }
      ],
      'stakeholder-brief': [
        {
          title: '戦略的計画',
          desc: 'チームのスケーリングや市場イニシアチブの優先順位付けを含む長期的なビジョン。',
          tasks: [
            {
              title: 'Q3リソースマトリックスの最適化',
              desc: '市場投入までの時間を短縮するために、ストリーム間で主要な開発者を再配置します。',
              dod: ['リソースプランが承認された', 'リスクが軽減された'],
              ac: ['負荷分散が確認された']
            }
          ]
        }
      ],
      'design-review': [
        {
          title: 'Wave Design System v4.0',
          desc: 'ビジュアル言語の進化、トークンの集中化、プレミアムなマイクロインタラクション。',
          tasks: [
            {
              title: 'デザイントークンシステム',
              desc: 'カラー、スペーシング、タイポグラフィの変数ライブラリを開発します。',
              dod: ['Web/モバイル用JSONエクスポート', 'Figma同期'],
              ac: ['ダークモード対応']
            }
          ]
        }
      ],
      'retrospective': [
        {
          title: 'プロセス効率',
          desc: 'ベロシティ向上のためのルーチンタスクの自動化とコードレビューサイクルの加速。',
          tasks: [
            {
              title: 'コードレビュー自動パイプライン',
              desc: 'PRでのコードスタイルとテストカバレッジチェックのためのボット統合。',
              dod: ['エラー時にプッシュをブロック', 'レビュアーステータスのアクティブ化'],
              ac: ['時間を30%短縮']
            }
          ]
        }
      ]
    }
  }
};

export const t = derived(lang, ($lang) => {
  return (path: string, ...args: any[]) => {
    const keys = path.split('.');
    let current: any = translations[$lang] || translations['ru'];
    for (const key of keys) {
      if (current && typeof current === 'object') {
        current = current[key];
      } else {
        return path;
      }
    }
    if (typeof current === 'function') {
      return current(...args);
    }
    return current || path;
  };
});
