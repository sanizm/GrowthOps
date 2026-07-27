namespace GrowthOps.Api.Dtos;

public record CreateGoalRequest(
    string Title,
    string? Description,
    string GoalType,          // "Metric" or "TaskBased"
    DateOnly TargetDate,
    DateOnly? StartDate,

    decimal? StartValue,
    decimal? TargetValue,
    string? Unit,
    bool? IsIncrease
);