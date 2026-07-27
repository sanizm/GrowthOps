namespace GrowthOps.Api.Dtos;

public record GoalStreakDto(
    int GoalId,
    int CurrentStreak,
    int LongestStreak,
    int TotalActiveDays
);
