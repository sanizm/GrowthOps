import { useState } from "react";
import type { CreateProgressEntryRequest } from "../api/progressEntries";

type AddProgressEntryFormProps = {
  goalType: string;
  onCancel: () => void;
  onCreate: (request: CreateProgressEntryRequest) => Promise<void>;
};

export default function AddProgressEntryForm({
  goalType,
  onCancel,
  onCreate,
}: AddProgressEntryFormProps) {
  const [value, setValue] = useState("");
  const [countDelta, setCountDelta] = useState("");
  const [loggedAtUtc, setLoggedAtUtc] = useState("");
  const [note, setNote] = useState("");
  const [error, setError] = useState("");

  const isMetric = goalType === "Metric";

  async function handleSubmit() {
    try {
      setError("");

      const request = {
        loggedDate: loggedAtUtc || null,
        value: isMetric && value ? Number(value) : null,
        countDelta: !isMetric && countDelta ? Number(countDelta) : null,
        note: note || null,
      };

      await onCreate(request);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to add entry");
    }
  }

  return (
    <div className="mb-5 rounded-xl border border-slate-200 bg-slate-50 p-4">
      <div className="grid gap-4 md:grid-cols-2">
        <div>
          <label className="text-sm font-medium text-slate-700">
            {isMetric ? "Value" : "Count Completed"}
          </label>
          <input
            type="number"
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
            value={isMetric ? value : countDelta}
            onChange={(e) =>
              isMetric
                ? setValue(e.target.value)
                : setCountDelta(e.target.value)
            }
            placeholder={isMetric ? "Example: 78.5" : "Example: 2"}
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700">
            Logged Date
          </label>
          <input
            type="date"
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
            value={loggedAtUtc}
            onChange={(e) => setLoggedAtUtc(e.target.value)}
          />
        </div>

        <div className="md:col-span-2">
          <label className="text-sm font-medium text-slate-700">Note</label>
          <input
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2"
            value={note}
            onChange={(e) => setNote(e.target.value)}
            placeholder="Optional note"
          />
        </div>
      </div>

      {error && (
        <p className="mt-3 rounded-lg bg-red-50 p-3 text-sm text-red-700">
          {error}
        </p>
      )}

      <div className="mt-4 flex gap-3">
        <button
          onClick={handleSubmit}
          className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
        >
          Save Entry
        </button>

        <button
          onClick={onCancel}
          className="rounded-lg border border-slate-300 px-4 py-2 text-sm text-slate-700 hover:bg-white"
        >
          Cancel
        </button>
      </div>
    </div>
  );
}
