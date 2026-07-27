namespace GrowthOps.Api.Dtos;

public record LoginRequest(
    string Email,
    string Password
);