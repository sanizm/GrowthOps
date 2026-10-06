using GrowthOps.Api.Data;
using GrowthOps.Api.Dtos;
using GrowthOps.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace GrowthOps.Api.Services;

public class GoalService
{
  // private readonly List<GoalDto> _goals = new()
  //   {
  //       new(1, "Learn ASP.NET Core"),
  //       new(2, "Build GrowthOps API")
  //   };

  private readonly GrowthOpsDbContext _db;

  public GoalService(GrowthOpsDbContext db)
  {
    _db = db;
  }

  // public List<GoalDto> GetAll() => _goals;

  public async Task<IReadOnlyList<GoalDto>> GetAllAsync(int userId, CancellationToken cancellationToken)
  {
    return await _db.Goals
    .Where(g => g.UserId == userId)
    .OrderBy(g => g.Id)
    .Select(g => new GoalDto(
        g.Id,
        g.Title,
        g.Description,
        g.GoalType.ToString(),
        g.StartDate,
        g.TargetDate,
        g.CreatedAtUtc,
        g.StartValue,
        g.TargetValue,
        g.Unit,
        g.IsIncrease
    ))
    .ToListAsync(cancellationToken);
  }
  // public GoalDto? GetById(int id) => _goals.FirstOrDefault(g => g.Id == id);

  public async Task<GoalDto?> GetByIdAsync(int id, int userId)
  {
    return await _db.Goals
    .Where(g => g.Id == id && g.UserId == userId)
    .Select(g => new GoalDto(
        g.Id,
        g.Title,
        g.Description,
        g.GoalType.ToString(),
        g.StartDate,
        g.TargetDate,
        g.CreatedAtUtc,
        g.StartValue,
        g.TargetValue,
        g.Unit,
        g.IsIncrease
    ))
    .FirstOrDefaultAsync();
  }

  public async Task<bool> DeleteAsync(int id, int userId)
  {
    var goal = await _db.Goals
        .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

    if (goal is null)
      return false;

    _db.Goals.Remove(goal);
    await _db.SaveChangesAsync();

    return true;
  }

  // public GoalDto Create(string title)
  // {
  //   var nextId = _goals.Count == 0 ? 1 : _goals.Max(g => g.Id) + 1;
  //   var goal = new GoalDto(nextId, title);
  //   _goals.Add(goal);
  //   return goal;
  // }
  public async Task<GoalDto> CreateAsync(CreateGoalRequest request, int userId)
  {
    if (!Enum.TryParse<GoalType>(request.GoalType, ignoreCase: true, out var goalType))
      throw new ArgumentException("Invalid GoalType. Use 'Metric' or 'TaskBased'.");

    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var startDate = request.StartDate ?? today;
    var entity = new Goal
    {
      Title = request.Title.Trim(),
      Description = request.Description,
      GoalType = goalType,
      StartDate = startDate,
      TargetDate = request.TargetDate,

      StartValue = request.StartValue,
      TargetValue = request.TargetValue,
      Unit = request.Unit,
      IsIncrease = request.IsIncrease,

      UserId = userId
    };
    _db.Goals.Add(entity);
    await _db.SaveChangesAsync();

    return new GoalDto(
        entity.Id,
        entity.Title,
        entity.Description,
        entity.GoalType.ToString(),
        entity.StartDate,
        entity.TargetDate,
        entity.CreatedAtUtc,
        entity.StartValue,
        entity.TargetValue,
        entity.Unit,
        entity.IsIncrease
    );
  }
}
