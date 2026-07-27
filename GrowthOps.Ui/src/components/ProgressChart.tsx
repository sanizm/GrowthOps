import {
  CartesianGrid,
  Line,
  LineChart,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import type { GoalChartPointResponse } from "../api/analytics";

type ProgressChartProps = {
  data: GoalChartPointResponse[];
  unit?: string | null;
};

export default function ProgressChart({ data, unit }: ProgressChartProps) {
  const chartData = data.map((point) => ({
    label: point.date.slice(5),
    value: point.value,
  }));

  if (chartData.length === 0) {
    return (
      <div className="rounded-xl border border-dashed border-slate-300 p-6 text-center text-slate-600">
        No chart data yet. Add progress entries to see your trend.
      </div>
    );
  }

  return (
    <div className="w-full overflow-x-auto rounded-xl border border-slate-200 bg-white p-4">
      <div className="flex justify-center">
        <LineChart
          width={760}
          height={280}
          data={chartData}
          margin={{ top: 10, right: 40, left: 10, bottom: 25 }}
        >
          <CartesianGrid strokeDasharray="3 3" />

          <XAxis
            dataKey="label"
            interval={0}
            tick={{ fontSize: 11 }}
            angle={-20}
            textAnchor="end"
            height={55}
            padding={{ left: 20, right: 30 }}
          />

          <YAxis width={40} />

          <Tooltip
            formatter={(value) => [`${value} ${unit ?? ""}`, "Progress"]}
          />

          <Line
            type="monotone"
            dataKey="value"
            strokeWidth={2}
            dot={{ r: 4 }}
          />
        </LineChart>
      </div>
    </div>
  );
}
