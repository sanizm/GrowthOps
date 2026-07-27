namespace GrowthOps.Api.Models;

public class ProgressEntry
{
  public int Id { get; set; }

  public int GoalId { get; set; }
  public Goal Goal { get; set; } = null!;

  public DateOnly LoggedDate { get; set; }

  public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

  // Metric-based check-in (e.g., weight = 79.2)
  public decimal? Value { get; set; }

  // Task-count check-in (e.g., completed 2 tasks today)
  public int? CountDelta { get; set; }

  public string? Note { get; set; }
}