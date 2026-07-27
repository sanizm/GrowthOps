namespace GrowthOps.Api.Dtos;

public record GoalDto(
    int Id,
    string Title,
    string? Description,
    string GoalType,
    DateOnly StartDate,
    DateOnly TargetDate,
    DateTime CreatedAtUtc,

    decimal? StartValue,
    decimal? TargetValue,
    string? Unit,
    bool? IsIncrease
);