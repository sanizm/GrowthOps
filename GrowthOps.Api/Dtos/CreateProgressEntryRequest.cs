namespace GrowthOps.Api.Dtos;

public record CreateProgressEntryRequest(
    DateOnly? LoggedDate,
    decimal? Value,
    int? CountDelta,
    string? Note
);