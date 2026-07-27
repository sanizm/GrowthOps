using GrowthOps.Api.Data;
using GrowthOps.Api.Dtos;
using GrowthOps.Api.Models;
using Microsoft.EntityFrameworkCore;

public class GoalProgressService
{
  private readonly GrowthOpsDbContext _db;

  public GoalProgressService(GrowthOpsDbContext db)
  {
    _db = db;
  }

  public async Task<GoalProgressSummaryDto?> GetSummaryAsync(int goalId, int userId)
  {
    var goal = await _db.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
    if (goal is null) return null;

    var now = DateTime.UtcNow;

    var daysTotal = goal.TargetDate.DayNumber - goal.StartDate.DayNumber;
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var daysRemaining = Math.Max(0, goal.TargetDate.DayNumber - today.DayNumber);
    if (goal.GoalType == GoalType.Metric)
    {

      // Latest metric value (if no entries, fall back to StartValue)
      var latestValue = await _db.ProgressEntries
          .Where(e => e.GoalId == goalId && e.Value.HasValue)
          .OrderByDescending(e => e.CreatedAtUtc)
          .Select(e => e.Value)
          .FirstOrDefaultAsync();

      var current = latestValue ?? goal.StartValue;

      // Need StartValue, TargetValue, IsIncrease for metric percent
      if (!goal.StartValue.HasValue || !goal.TargetValue.HasValue || !goal.IsIncrease.HasValue || !current.HasValue)
      {
        return new GoalProgressSummaryDto(
            goal.Id,
            goal.GoalType.ToString(),
            current,
            null,
            goal.TargetValue,
            goal.StartValue,
            goal.IsIncrease,
            0m,
            goal.StartDate,
            goal.TargetDate,
            daysTotal,
            daysRemaining
        );
      }
      var start = goal.StartValue.Value;
      var target = goal.TargetValue.Value;

      // Avoid divide-by-zero
      if (start == target)
      {
        var percent = current.Value == target ? 100m : 0m;
        return Build(goal, current, null, percent, daysTotal, daysRemaining);
      }
      decimal raw;
      if (goal.IsIncrease.Value)
        raw = (current.Value - start) / (target - start);
      else
        raw = (start - current.Value) / (start - target);

      var percentClamped = ClampPercent(raw * 100m);
      return Build(goal, current, null, percentClamped, daysTotal, daysRemaining);
    }

    // TaskBased (count-based V1): progress = sum(CountDelta) / TargetValue
    var totalCount = await _db.ProgressEntries
        .Where(e => e.GoalId == goalId && e.CountDelta.HasValue)
        .SumAsync(e => (int?)e.CountDelta) ?? 0;

    decimal percentTask = 0m;
    if (goal.TargetValue.HasValue && goal.TargetValue.Value > 0)
    {
      percentTask = ClampPercent((decimal)totalCount / goal.TargetValue.Value * 100m);
    }

    return new GoalProgressSummaryDto(
        goal.Id,
        goal.GoalType.ToString(),
        null,
        totalCount,
        goal.TargetValue,
        goal.StartValue,
        goal.IsIncrease,
        percentTask,
        goal.StartDate,
        goal.TargetDate,
        daysTotal,
        daysRemaining
    );
  }

  private static GoalProgressSummaryDto Build(Goal goal, decimal? currentValue, int? currentCount, decimal percent, int daysTotal, int daysRemaining)
    => new(
        goal.Id,
        goal.GoalType.ToString(),
        currentValue,
        currentCount,
        goal.TargetValue,
        goal.StartValue,
        goal.IsIncrease,
        percent,
        goal.StartDate,
        goal.TargetDate,
        daysTotal,
        daysRemaining
    );

  private static decimal ClampPercent(decimal p) => p < 0 ? 0 : (p > 100 ? 100 : p);


}