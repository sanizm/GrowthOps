namespace GrowthOps.Api.Dtos;

public record GoalAnalyticsDto(
    GoalDto Goal,
    GoalProgressSummaryDto Progress,
    GoalStreakDto Streak,
    int TotalEntries
);
