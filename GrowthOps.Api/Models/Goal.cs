using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GrowthOps.Api.Models;

public class Goal
{
  public int Id { get; set; }

  // Multi-user later; for now we'll keep a placeholder
  public int UserId { get; set; } = 1;
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }

  public GoalType GoalType { get; set; }

  // “Expected date / deadline”
  public DateOnly StartDate { get; set; }
  public DateOnly TargetDate { get; set; }

  public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

  // Metric-only fields (null for TaskBased)
  public decimal? StartValue { get; set; }
  public decimal? TargetValue { get; set; }
  public string? Unit { get; set; } // e.g. "kg", "$", "hours"
  public bool? IsIncrease { get; set; } // true=increase towards target, false=decrease

  public List<ProgressEntry> Entries { get; set; } = new();
}