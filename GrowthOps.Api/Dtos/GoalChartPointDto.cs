namespace GrowthOps.Api.Dtos;

public record GoalChartPointDto(
    DateOnly Date,
    decimal Value
);