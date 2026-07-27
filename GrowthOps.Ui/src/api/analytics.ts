import { apiFetch } from "./client";

export type GoalProgressResponse = {
  goalId: number;
  goalType: string;
  currentValue: number | null;
  currentCount: number | null;
  targetValue: number | null;
  startValue: number | null;
  isIncrease: boolean | null;
  progressPercent: number;
  startDate: string;
  targetDate: string;
  daysTotal: number;
  daysRemaining: number;
};

export type GoalStreakResponse = {
  goalId: number;
  currentStreak: number;
  longestStreak: number;
  totalActiveDays: number;
};

export type GoalChartPointResponse = {
  date: string;
  value: number;
};

export type GoalAnalyticsResponse = {
  goal: {
    id: number;
    title: string;
    description: string | null;
    goalType: string;
    startDate: string;
    targetDate: string;
    createdAtUtc: string;
    startValue: number | null;
    targetValue: number | null;
    unit: string | null;
    isIncrease: boolean | null;
  };
  progress: GoalProgressResponse | null;
  streak: GoalStreakResponse | null;
  totalEntries: number;
};

export function getGoalSummary(goalId: number) {
  return apiFetch<GoalProgressResponse>(`/goals/${goalId}/summary`);
}

export function getGoalStreak(goalId: number) {
  return apiFetch<GoalStreakResponse>(`/goals/${goalId}/streak`);
}

export function getGoalChart(goalId: number) {
  return apiFetch<GoalChartPointResponse[]>(`/goals/${goalId}/chart`);
}

export function getGoalAnalytics(goalId: number) {
  return apiFetch<GoalAnalyticsResponse>(`/goals/${goalId}/analytics`);
}
