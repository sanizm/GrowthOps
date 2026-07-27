export function formatDate(date?: string | null) {
  if (!date) return "N/A";

  const [year, month, day] = date.split("-");

  if (!year || !month || !day) return date;

  return `${month}/${day}/${year}`;
}
