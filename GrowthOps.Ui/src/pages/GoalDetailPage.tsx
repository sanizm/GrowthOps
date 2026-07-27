import { useEffect, useState } from "react";
import type { GoalResponse } from "../api/goals";
import { formatDate } from "../utils/date";
import {
  createProgressEntry,
  getProgressEntries,
  type CreateProgressEntryRequest,
  type ProgressEntryResponse,
} from "../api/progressEntries";
import AddProgressEntryForm from "../components/AddProgressEntryForm";
import {
  getGoalAnalytics,
  getGoalChart,
  type GoalAnalyticsResponse,
  type GoalChartPointResponse,
} from "../api/analytics";
import ProgressChart from "../components/ProgressChart";

type GoalDetailPageProps = {
  goal: GoalResponse;
  onClose: () => void;
};

export default function GoalDetailPage({ goal, onClose }: GoalDetailPageProps) {
  const [entries, setEntries] = useState<ProgressEntryResponse[]>([]);
  const [analytics, setAnalytics] = useState<GoalAnalyticsResponse | null>(
    null,
  );
  const [chartData, setChartData] = useState<GoalChartPointResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [error, setError] = useState("");

  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 10;

  useEffect(() => {
    async function loadGoalDetail() {
      try {
        setIsLoading(true);
        setError("");

        const entriesData = await getProgressEntries(goal.id, page, pageSize);
        setEntries(entriesData.items);
        setTotalPages(entriesData.totalPages);

        const analyticsData = await getGoalAnalytics(goal.id);
        setAnalytics(analyticsData);

        const chart = await getGoalChart(goal.id);
        setChartData(chart);
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Failed to load goal data",
        );
      } finally {
        setIsLoading(false);
      }
    }

    loadGoalDetail();
  }, [goal.id, page]);

  useEffect(() => {
    async function loadGoalDetail() {
      try {
        setIsLoading(true);
        setError("");

        const entriesData = await getProgressEntries(goal.id, page, pageSize);
        setEntries(entriesData.items);
        setTotalPages(entriesData.totalPages);

        const analyticsData = await getGoalAnalytics(goal.id);
        setAnalytics(analyticsData);
        console.log("Selected goal id:", goal.id);

        const chart = await getGoalChart(goal.id);
        console.log("Chart response:", chart);

        setChartData(chart);
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Failed to load goal data",
        );
      } finally {
        setIsLoading(false);
      }
    }

    loadGoalDetail();
  }, [goal.id, page]);

  async function handleCreateEntry(request: CreateProgressEntryRequest) {
    await createProgressEntry(goal.id, request);
    setShowForm(false);
    setPage(1);

    const entriesData = await getProgressEntries(goal.id, 1, pageSize);
    setEntries(entriesData.items);
    setTotalPages(entriesData.totalPages);

    const analyticsData = await getGoalAnalytics(goal.id);
    setAnalytics(analyticsData);

    const chart = await getGoalChart(goal.id);
    setChartData(chart);
  }

  return (
    <div className="mb-8 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
      <div className="mb-5 flex items-start justify-between">
        <div>
          <p className="text-sm font-semibold text-slate-500">Selected Goal</p>
          <h2 className="mt-1 text-2xl font-bold text-slate-900">
            {goal.title}
          </h2>
          <p className="mt-1 text-sm text-slate-500">{goal.goalType}</p>
        </div>

        <button
          onClick={onClose}
          className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm text-slate-700 hover:bg-slate-50"
        >
          Close
        </button>
      </div>

      {analytics && (
        <>
          <div className="mb-6 grid gap-4 md:grid-cols-4">
            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
              <p className="text-sm text-slate-500">Progress</p>
              <p className="mt-1 text-2xl font-bold text-slate-900">
                {Math.round(analytics.progress?.progressPercent ?? 0)}%
              </p>
            </div>

            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
              <p className="text-sm text-slate-500">Current</p>
              <p className="mt-1 text-2xl font-bold text-slate-900">
                {analytics.progress?.currentValue ??
                  analytics.progress?.currentCount ??
                  0}{" "}
                {goal.unit ?? ""}
              </p>
            </div>

            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
              <p className="text-sm text-slate-500">Days Remaining</p>
              <p className="mt-1 text-2xl font-bold text-slate-900">
                {analytics.progress?.daysRemaining ?? 0}
              </p>
            </div>

            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
              <p className="text-sm text-slate-500">Entries</p>
              <p className="mt-1 text-2xl font-bold text-slate-900">
                {analytics.totalEntries}
              </p>
            </div>
          </div>

          <div className="mb-6 grid gap-4 md:grid-cols-2">
            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
              <p className="text-sm text-slate-500">Current Streak</p>
              <p className="mt-1 text-2xl font-bold text-slate-900">
                {analytics.streak?.currentStreak ?? 0} days
              </p>
            </div>

            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
              <p className="text-sm text-slate-500">Longest Streak</p>
              <p className="mt-1 text-2xl font-bold text-slate-900">
                {analytics.streak?.longestStreak ?? 0} days
              </p>
            </div>
          </div>

          <div className="mb-6">
            <div className="mb-3 flex items-center justify-between">
              <h3 className="text-lg font-semibold text-slate-900">
                Progress Chart
              </h3>

              <p className="text-sm text-slate-500">
                Total active days: {analytics.streak?.totalActiveDays ?? 0}
              </p>
            </div>

            <ProgressChart data={chartData} unit={goal.unit} />
          </div>
        </>
      )}

      <div className="mb-5 flex justify-between">
        <h3 className="text-lg font-semibold text-slate-900">
          Progress Entries
        </h3>

        <button
          onClick={() => setShowForm(true)}
          className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
        >
          Add Entry
        </button>
      </div>

      {showForm && (
        <AddProgressEntryForm
          goalType={goal.goalType}
          onCancel={() => setShowForm(false)}
          onCreate={handleCreateEntry}
        />
      )}

      {isLoading && <p className="text-slate-600">Loading entries...</p>}

      {error && (
        <p className="rounded-lg bg-red-50 p-3 text-sm text-red-700">{error}</p>
      )}

      {!isLoading && !error && entries.length === 0 && (
        <div className="rounded-xl border border-dashed border-slate-300 p-6 text-center text-slate-600">
          No progress entries yet. Add your first update.
        </div>
      )}

      {!isLoading && !error && entries.length > 0 && (
        <>
          <div className="space-y-3">
            {entries.map((entry) => (
              <div
                key={entry.id}
                className="rounded-xl border border-slate-200 p-4"
              >
                <div className="flex justify-between">
                  <p className="font-medium text-slate-900">
                    {entry.value !== null
                      ? `${entry.value} ${goal.unit ?? ""}`
                      : `+${entry.countDelta} ${goal.unit ?? "tasks"}`}
                  </p>

                  <p className="text-sm text-slate-500">
                    {formatDate(entry.loggedDate)}
                  </p>
                </div>

                {entry.note && (
                  <p className="mt-2 text-sm text-slate-500">{entry.note}</p>
                )}
              </div>
            ))}
          </div>

          <div className="mt-5 flex items-center justify-between">
            <button
              disabled={page <= 1}
              onClick={() => setPage((current) => current - 1)}
              className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-50"
            >
              Previous
            </button>

            <p className="text-sm text-slate-500">
              Page {page} of {totalPages}
            </p>

            <button
              disabled={page >= totalPages}
              onClick={() => setPage((current) => current + 1)}
              className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-50"
            >
              Next
            </button>
          </div>
        </>
      )}
    </div>
  );
}
