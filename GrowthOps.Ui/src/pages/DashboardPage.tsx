import { useEffect, useState } from "react";
import {
  createGoal,
  getGoals,
  deleteGoal,
  type CreateGoalRequest,
  type GoalResponse,
} from "../api/goals";
import GoalCard from "../components/GoalCard";
import CreateGoalForm from "../components/CreateGoalForm";
import GoalDetailPage from "./GoalDetailPage";

type DashboardPageProps = {
  onLogout: () => void;
};

export default function DashboardPage({ onLogout }: DashboardPageProps) {
  const [goals, setGoals] = useState<GoalResponse[]>([]);
  const [selectedGoal, setSelectedGoal] = useState<GoalResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [showCreateForm, setShowCreateForm] = useState(false);

  async function loadGoals() {
    try {
      setError("");
      const data = await getGoals();
      setGoals(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load goals");
    } finally {
      setIsLoading(false);
    }
  }

  async function handleDeleteGoal(id: number) {
    const confirmed = window.confirm(
      "Are you sure you want to delete this goal? This will also delete its progress entries.",
    );

    if (!confirmed) return;

    try {
      await deleteGoal(id);

      if (selectedGoal?.id === id) {
        setSelectedGoal(null);
      }

      await loadGoals();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to delete goal");
    }
  }

  useEffect(() => {
    async function loadGoals() {
      try {
        setError("");
        const data = await getGoals();
        setGoals(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load goals");
      } finally {
        setIsLoading(false);
      }
    }
    loadGoals();
  }, []);

  async function handleCreateGoal(request: CreateGoalRequest) {
    await createGoal(request);
    setShowCreateForm(false);
    await loadGoals();
  }

  return (
    <div className="min-h-screen bg-slate-100 px-6 py-8">
      <div className="mx-auto max-w-5xl">
        <div className="mb-8 flex items-center justify-between">
          <div>
            <p className="text-sm font-semibold text-slate-500">GrowthOps</p>
            <h1 className="mt-1 text-3xl font-bold text-slate-900">
              Your Goals
            </h1>
            <p className="mt-2 text-slate-600">
              Track your metric and task-based goals in one place.
            </p>
          </div>

          <div className="flex gap-3">
            <button
              onClick={() => setShowCreateForm(true)}
              className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
            >
              Create Goal
            </button>

            <button
              onClick={onLogout}
              className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-white"
            >
              Logout
            </button>
          </div>
        </div>

        {showCreateForm && (
          <CreateGoalForm
            onCancel={() => setShowCreateForm(false)}
            onCreate={handleCreateGoal}
          />
        )}

        {isLoading && (
          <div className="rounded-2xl bg-white p-6 text-slate-600 shadow-sm">
            Loading goals...
          </div>
        )}

        {error && (
          <div className="rounded-2xl border border-red-200 bg-red-50 p-6 text-red-700">
            {error}
          </div>
        )}

        {!isLoading && !error && goals.length === 0 && !showCreateForm && (
          <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center shadow-sm">
            <h2 className="text-xl font-semibold text-slate-900">
              No goals yet
            </h2>
            <p className="mt-2 text-slate-600">
              Create your first goal to start tracking progress, streaks, and
              analytics.
            </p>
            <button
              onClick={() => setShowCreateForm(true)}
              className="mt-5 rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
            >
              Create your first goal
            </button>
          </div>
        )}
        {selectedGoal && (
          <GoalDetailPage
            key={selectedGoal.id}
            goal={selectedGoal}
            onClose={() => setSelectedGoal(null)}
          />
        )}
        {!isLoading && !error && goals.length > 0 && (
          <div className="grid gap-4 md:grid-cols-2">
            {goals.map((goal) => (
              <GoalCard
                key={goal.id}
                goal={goal}
                onDelete={handleDeleteGoal}
                onSelect={setSelectedGoal}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
