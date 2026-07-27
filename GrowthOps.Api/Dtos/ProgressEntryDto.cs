namespace GrowthOps.Api.Dtos;

public record ProgressEntryDto(
    int Id,
    int GoalId,
    DateOnly LoggedDate,
    DateTime CreatedAtUtc,
    decimal? Value,
    int? CountDelta,
    string? Note
);