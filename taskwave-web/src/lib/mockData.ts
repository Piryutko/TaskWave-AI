export interface Task {
  title: string;
  desc: string;
  dod: string[];
  ac: string[];
}

export interface Epic {
  title: string;
  desc: string;
  tasks: Task[];
}

export const mockAudioFiles: Record<string, string> = {
  "daily-standup": "daily_sync_dev_team_1403.wav",
  "stakeholder-brief": "stakeholder_briefing_q1_mvp.mp3",
  "design-review": "design_review_wave_system_final.wav",
  "retrospective": "retro_sprint_22_efficiency.mp3"
};

// These are now handled by the i18n system as translation keys
export const mockResultKeys = ["daily-standup", "stakeholder-brief", "design-review", "retrospective"];
