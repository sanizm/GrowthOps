import { apiFetch } from "./client";

export type ProgressEntryResponse = {
  id: number;
  goalId: number;
  loggedDate: string;
  createdAtUtc: string;
  value: number | null;
  countDelta: number | null;
  note: string | null;
};

export type CreateProgressEntryRequest = {
  loggedDate?: string | null;
  value?: number | null;
  countDelta?: number | null;
  note?: string | null;
};

export type PagedResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export function getProgressEntries(goalId: number, page = 1, pageSize = 10) {
  return apiFetch<PagedResult<ProgressEntryResponse>>(
    `/goals/${goalId}/entries?page=${page}&pageSize=${pageSize}`,
  );
}

export function createProgressEntry(
  goalId: number,
  request: CreateProgressEntryRequest,
) {
  return apiFetch<ProgressEntryResponse>(`/goals/${goalId}/entries`, {
    method: "POST",
    body: JSON.stringify(request),
  });
}
