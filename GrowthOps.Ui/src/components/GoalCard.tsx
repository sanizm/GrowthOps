import type { GoalResponse } from "../api/goals";
import { formatDate } from "../utils/date";
type GoalCardProps = {
  goal: GoalResponse;
  onDelete: (id: number) => void;
  onSelect: (goal: GoalResponse) => void;
};

export default function GoalCard({ goal, onDelete, onSelect }: GoalCardProps) {
  return (
    <div
      onClick={() => onSelect(goal)}
      className="cursor-pointer rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:border-slate-400 hover:shadow-md"
    >
      <div className="flex items-start justify-between gap-4">
        <div>
          <h3 className="text-lg font-semibold text-slate-900">{goal.title}</h3>

          {goal.description && (
            <p className="mt-1 text-sm text-slate-500">{goal.description}</p>
          )}
        </div>

        <span className="rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-700">
          {goal.goalType}
        </span>
      </div>

      <div className="mt-4 grid grid-cols-2 gap-3 text-sm">
        <div>
          <p className="text-slate-500">Start Date</p>
          <p className="font-medium text-slate-900">
            {formatDate(goal.startDate)}
          </p>
        </div>

        <div>
          <p className="text-slate-500">Target Date</p>
          <p className="font-medium text-slate-900">
            {formatDate(goal.targetDate)}
          </p>
        </div>

        <div>
          <p className="text-slate-500">
            {goal.goalType === "Metric" ? "Start Value" : "Starting Count"}
          </p>
          <p className="font-medium text-slate-900">
            {goal.startValue ?? "N/A"} {goal.unit ?? ""}
          </p>
        </div>

        <div>
          <p className="text-slate-500">
            {goal.goalType === "Metric" ? "Target Value" : "Target Count"}
          </p>
          <p className="font-medium text-slate-900">
            {goal.targetValue ?? "N/A"} {goal.unit ?? ""}
          </p>
        </div>
      </div>

      <div className="mt-5 flex items-center justify-between">
        <p className="text-xs text-slate-400">Click to view progress</p>

        <button
          onClick={(e) => {
            e.stopPropagation();
            onDelete(goal.id);
          }}
          className="rounded-lg border border-red-200 px-3 py-1.5 text-sm font-medium text-red-600 hover:bg-red-50"
        >
          Delete
        </button>
      </div>
    </div>
  );
}
