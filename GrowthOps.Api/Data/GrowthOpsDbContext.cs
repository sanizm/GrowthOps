using GrowthOps.Api.Dtos;
using GrowthOps.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GrowthOps.Api.Data;

public class GrowthOpsDbContext : DbContext
{
  public GrowthOpsDbContext(DbContextOptions<GrowthOpsDbContext> options) : base(options) { }

  public DbSet<Goal> Goals => Set<Goal>();

  public DbSet<ProgressEntry> ProgressEntries => Set<ProgressEntry>();

  public DbSet<User> Users => Set<User>();


}