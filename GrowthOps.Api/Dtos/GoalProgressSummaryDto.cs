namespace GrowthOps.Api.Dtos;

public record GoalProgressSummaryDto(
    int GoalId,
    string GoalType,

    // Metric: CurrentValue used; Task: CurrentCount used
    decimal? CurrentValue,
    int? CurrentCount,

    decimal? TargetValue,
    decimal? StartValue,
    bool? IsIncrease,

    decimal ProgressPercent,

    DateOnly StartDate,
    DateOnly TargetDate,
    int DaysTotal,
    int DaysRemaining
);
