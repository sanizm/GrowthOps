import { useState } from "react";
import type { CreateGoalRequest } from "../api/goals";

type CreateGoalFormProps = {
  onCancel: () => void;
  onCreate: (request: CreateGoalRequest) => Promise<void>;
};

export default function CreateGoalForm({
  onCancel,
  onCreate,
}: CreateGoalFormProps) {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [goalType, setGoalType] = useState<"Metric" | "TaskBased">("Metric");
  const [targetDateUtc, setTargetDateUtc] = useState("");
  const [startValue, setStartValue] = useState("");
  const [targetValue, setTargetValue] = useState("");
  const [unit, setUnit] = useState("kg");
  const [isIncrease, setIsIncrease] = useState(false);
  const [error, setError] = useState("");

  function todayDateOnly() {
    return new Date().toISOString().split("T")[0];
  }
  async function handleSubmit() {
    try {
      setError("");

      if (!title.trim()) {
        setError("Title is required.");
        return;
      }

      if (!targetDateUtc) {
        setError("Target date is required.");
        return;
      }

      const request: CreateGoalRequest = {
        title,
        description: description || null,
        goalType,
        startDate: todayDateOnly(),
        targetDate: targetDateUtc,
        startValue: startValue ? Number(startValue) : null,
        targetValue: targetValue ? Number(targetValue) : null,
        unit: unit || null,
        isIncrease,
      };

      await onCreate(request);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create goal");
    }
  }

  return (
    <div className="mb-8 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
      <h2 className="text-xl font-semibold text-slate-900">Create Goal</h2>

      <div className="mt-5 grid gap-4 md:grid-cols-2">
        <div className="md:col-span-2">
          <label className="text-sm font-medium text-slate-700">Title</label>
          <input
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-400"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Reduce weight to 75kg"
          />
        </div>

        <div className="md:col-span-2">
          <label className="text-sm font-medium text-slate-700">
            Description
          </label>
          <input
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-400"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Optional description"
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700">
            Goal Type
          </label>
          <select
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-400"
            value={goalType}
            onChange={(e) => {
              const selectedType = e.target.value as "Metric" | "TaskBased";
              setGoalType(selectedType);

              if (selectedType === "TaskBased") {
                setUnit("tasks");
              } else {
                setUnit("");
              }
            }}
          >
            <option value="Metric">Metric</option>
            <option value="TaskBased">Task Based</option>
          </select>
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700">
            Target Date
          </label>
          <input
            type="date"
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-400"
            value={targetDateUtc}
            onChange={(e) => setTargetDateUtc(e.target.value)}
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700">
            Start Value
          </label>
          <input
            type="number"
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-400"
            value={startValue}
            onChange={(e) => setStartValue(e.target.value)}
            placeholder={goalType === "Metric" ? "Example: 80" : "Example: 0"}
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700">
            Target Value
          </label>
          <input
            type="number"
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-400"
            value={targetValue}
            onChange={(e) => setTargetValue(e.target.value)}
            placeholder={goalType === "Metric" ? "Example: 75" : "Example: 50"}
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700">Unit</label>
          <input
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-400"
            value={unit}
            onChange={(e) => setUnit(e.target.value)}
            placeholder={
              goalType === "Metric" ? "kg, km, $, hours, %" : "tasks"
            }
          />
        </div>

        <div className="flex items-end">
          <label className="flex items-center gap-2 text-sm text-slate-700">
            <input
              type="checkbox"
              checked={isIncrease}
              onChange={(e) => setIsIncrease(e.target.checked)}
            />
            Increase goal?
          </label>
        </div>
      </div>

      {error && (
        <p className="mt-4 rounded-lg bg-red-50 p-3 text-sm text-red-700">
          {error}
        </p>
      )}

      <div className="mt-6 flex gap-3">
        <button
          onClick={handleSubmit}
          className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
        >
          Save Goal
        </button>

        <button
          onClick={onCancel}
          className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          Cancel
        </button>
      </div>
    </div>
  );
}
