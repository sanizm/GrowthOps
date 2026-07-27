using GrowthOps.Api.Data;
using GrowthOps.Api.Dtos;
using GrowthOps.Api.Models;
using GrowthOps.Api.Services;

public class GoalAnalyticsService
{
  private readonly GoalService _goalService;
  private readonly GoalProgressService _progressService;
  private readonly GoalStreakService _streakService;
  private readonly ProgressEntryService _entryService;

  public GoalAnalyticsService(
      GoalService goalService,
      GoalProgressService progressService,
      GoalStreakService streakService,
      ProgressEntryService entryService)
  {
    _goalService = goalService;
    _progressService = progressService;
    _streakService = streakService;
    _entryService = entryService;
  }

  public async Task<GoalAnalyticsDto?> GetAsync(int goalId, int userId)
  {
    var goal = await _goalService.GetByIdAsync(goalId, userId);

    if (goal == null) return null;

    var progress = await _progressService.GetSummaryAsync(goalId, userId);
    var streak = await _streakService.GetStreakAsync(goalId, userId);
    var entries = await _entryService.GetForGoalAsync(goalId, userId);

    if (progress is null || streak is null) return null;

    return new GoalAnalyticsDto(
        goal,
        progress,
        streak,
        entries?.TotalCount ?? 0
    );
  }


}