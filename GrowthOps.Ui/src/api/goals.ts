import { apiFetch } from "./client";

export type GoalResponse = {
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

export type CreateGoalRequest = {
  title: string;
  description?: string | null;
  goalType: string;
  startDate?: string | null;
  targetDate: string;
  startValue?: number | null;
  targetValue?: number | null;
  unit?: string | null;
  isIncrease?: boolean | null;
};

export function getGoals() {
  return apiFetch<GoalResponse[]>("/goals");
}

export function createGoal(request: CreateGoalRequest) {
  return apiFetch<GoalResponse>("/goals", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function getGoalsById(Id: number) {
  return apiFetch<GoalResponse>(`/goals/${Id}`);
}

export function deleteGoal(id: number) {
  return apiFetch<void>(`/goals/${id}`, {
    method: "DELETE",
  });
}
