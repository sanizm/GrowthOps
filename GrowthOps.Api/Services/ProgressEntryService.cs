using GrowthOps.Api.Data;
using GrowthOps.Api.Dtos;
using GrowthOps.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace GrowthOps.Api.Services;

public class ProgressEntryService
{

  private readonly GrowthOpsDbContext _db;

  public ProgressEntryService(GrowthOpsDbContext db)
  {
    _db = db;
  }

  public async Task<PagedResultDto<ProgressEntryDto>?> GetForGoalAsync(
      int goalId,
      int userId,
      int page = 1,
      int pageSize = 10)
  {
    var goalExists = await _db.Goals
        .AnyAsync(g => g.Id == goalId && g.UserId == userId);

    if (!goalExists)
      return null;

    page = Math.Max(1, page);
    pageSize = Math.Clamp(pageSize, 1, 50);

    var query = _db.ProgressEntries
        .Where(e => e.GoalId == goalId)
        .OrderByDescending(e => e.LoggedDate)
        .ThenByDescending(e => e.CreatedAtUtc)
        .ThenByDescending(e => e.Id);

    var totalCount = await query.CountAsync();

    var entries = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(e => new ProgressEntryDto(
            e.Id,
            e.GoalId,
            e.LoggedDate,
            e.CreatedAtUtc,
            e.Value,
            e.CountDelta,
            e.Note
        ))
        .ToListAsync();

    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

    return new PagedResultDto<ProgressEntryDto>(
        entries,
        page,
        pageSize,
        totalCount,
        totalPages
    );
  }

  public async Task<ProgressEntryDto?> CreateAsync(int goalId, CreateProgressEntryRequest request, int userId)
  {
    var goal = await _db.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
    if (goal is null) return null;
    var loggedDate = request.LoggedDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

    if (loggedDate < goal.StartDate)
      throw new ArgumentException("Progress entry cannot be before goal start date.");

    if (loggedDate > DateOnly.FromDateTime(DateTime.UtcNow))
      throw new ArgumentException("Progress entry cannot be in the future.");
    var progressEntryExists = await _db.ProgressEntries.AnyAsync(pe =>
        pe.GoalId == goalId &&
        pe.LoggedDate == loggedDate &&
        pe.Value == request.Value &&
        pe.CountDelta == request.CountDelta &&
        pe.Note == request.Note);

    if (progressEntryExists)
    {
      throw new ArgumentException("Duplicate progress entry detected.");
    }
    var hasValue = request.Value.HasValue;
    var hasCount = request.CountDelta.HasValue;

    if (!hasValue && !hasCount)
      throw new ArgumentException("Provide either Value (metric) or CountDelta (task count).");

    if (hasValue && hasCount)
      throw new ArgumentException("Provide only one: Value OR CountDelta, not both.");

    // Validate against the actual goal type stored in DB
    if (goal.GoalType == GoalType.Metric)
    {
      if (!hasValue)
        throw new ArgumentException("Metric goals require Value.");

      if (hasCount)
        throw new ArgumentException("Metric goals do not allow CountDelta.");
    }
    else if (goal.GoalType == GoalType.TaskBased)
    {
      if (!hasCount)
        throw new ArgumentException("Task-based goals require CountDelta.");

      if (hasValue)
        throw new ArgumentException("Task-based goals do not allow Value.");

      if (request.CountDelta!.Value <= 0)
        throw new ArgumentException("CountDelta must be greater than 0.");
    }
    var entry = new ProgressEntry
    {
      GoalId = goalId,
      LoggedDate = loggedDate,
      Value = request.Value,
      CountDelta = request.CountDelta,
      Note = request.Note
    };

    _db.ProgressEntries.Add(entry);
    await _db.SaveChangesAsync();

    return new ProgressEntryDto(
        entry.Id,
        entry.GoalId,
        entry.LoggedDate,
        entry.CreatedAtUtc,
        entry.Value,
        entry.CountDelta,
        entry.Note
    );

  }

  public async Task<IReadOnlyList<GoalChartPointDto>> GetChartDataAsync(int goalId, int userId)
  {
    var goal = await _db.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
    if (goal is null) return [];

    List<GoalChartPointDto> chart;

    // METRIC GOALS
    if (goal.GoalType == GoalType.Metric)
    {
      var entries = await _db.ProgressEntries
    .Where(e => e.GoalId == goalId && e.Value != null)
    .OrderBy(e => e.LoggedDate)
    .ThenBy(e => e.CreatedAtUtc)
    .ToListAsync();

      return entries
          .GroupBy(e => e.LoggedDate)
          .Select(g =>
          {
            var latest = g
          .OrderByDescending(e => e.CreatedAtUtc)
          .ThenByDescending(e => e.Id)
          .First();

            return new GoalChartPointDto(g.Key, latest.Value!.Value);
          })
          .OrderBy(x => x.Date)
          .ToList();
    }

    // TASK-BASED GOALS
    else
    {
      var entries = await _db.ProgressEntries
          .Where(e => e.GoalId == goalId && e.CountDelta != null)
          .OrderBy(e => e.CreatedAtUtc)
          .ToListAsync();

      var daily = entries
          .GroupBy(e => e.LoggedDate)
          .Select(g => new
          {
            Date = g.Key,
            Total = g.Sum(e => e.CountDelta!.Value)
          })
          .OrderBy(x => x.Date)
          .ToList();

      chart = new List<GoalChartPointDto>();
      int runningTotal = 0;

      foreach (var d in daily)
      {
        runningTotal += d.Total;

        chart.Add(new GoalChartPointDto(
            d.Date,
            runningTotal
        ));
      }
    }

    // Fill missing days between first and last known date
    if (chart.Count == 0) return chart;

    var filled = new List<GoalChartPointDto>();

    var start = chart.First().Date;
    var end = chart.Last().Date;

    var map = chart.ToDictionary(x => x.Date, x => x.Value);

    decimal lastValue = chart.First().Value;

    for (var date = start; date <= end; date = date.AddDays(1))
    {
      if (map.ContainsKey(date))
      {
        lastValue = map[date];
      }

      filled.Add(new GoalChartPointDto(date, lastValue));
    }

    return filled;
  }
}