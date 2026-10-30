using GrowthOps.Api.Data;
using GrowthOps.Api.Models;
using GrowthOps.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace GrowthOps.Api.Tests;

public class GoalServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsOnlyGoalsForRequestedUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<GrowthOpsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new GrowthOpsDbContext(options);

        db.Goals.AddRange(
            new Goal
            {
                UserId = 1,
                Title = "User 1 Goal",
                GoalType = GoalType.Metric,
                StartDate = new DateOnly(2026, 1, 1),
                TargetDate = new DateOnly(2026, 12, 31)
            },
            new Goal
            {
                UserId = 2,
                Title = "User 2 Goal",
                GoalType = GoalType.Metric,
                StartDate = new DateOnly(2026, 1, 1),
                TargetDate = new DateOnly(2026, 12, 31)
            }
        );

        await db.SaveChangesAsync();

        var service = new GoalService(db);

        // Act
        var result = await service.GetAllAsync(1, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("User 1 Goal", result[0].Title);
    }
}