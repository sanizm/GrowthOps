using System.Data.Common;
using GrowthOps.Api.Data;
using GrowthOps.Api.Dtos;
using Microsoft.EntityFrameworkCore;

public class GoalStreakService
{
  private readonly GrowthOpsDbContext _db;

  public GoalStreakService(GrowthOpsDbContext db)
  {
    _db = db;
  }

  public async Task<GoalStreakDto?> GetStreakAsync(int goalId, int userId)
  {
    var goalExists = await _db.Goals
        .AnyAsync(g => g.Id == goalId && g.UserId == userId);

    if (!goalExists)
      return null;

    var dates = await _db.ProgressEntries
        .Where(e => e.GoalId == goalId)
        .Select(e => e.LoggedDate)
        .Distinct()
        .OrderBy(d => d)
        .ToListAsync();

    if (dates.Count == 0)
      return new GoalStreakDto(goalId, 0, 0, 0);

    int longest = 1;
    int temp = 1;

    for (int i = 1; i < dates.Count; i++)
    {
      if (dates[i] == dates[i - 1].AddDays(1))
      {
        temp++;
        longest = Math.Max(longest, temp);
      }
      else
      {
        temp = 1;
      }
    }

    var dateSet = dates.ToHashSet();
    var today = DateOnly.FromDateTime(DateTime.UtcNow);

    int current = 0;
    var checkDate = today;

    while (dateSet.Contains(checkDate))
    {
      current++;
      checkDate = checkDate.AddDays(-1);
    }

    return new GoalStreakDto(
        goalId,
        current,
        longest,
        dates.Count
    );
  }
}