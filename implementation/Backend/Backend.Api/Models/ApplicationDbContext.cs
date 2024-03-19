using Backend.Api.Models.Schedules;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Models;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions options) : base(options) { }

  public DbSet<Associate> Associates => Set<Associate>();
  public DbSet<Schedule> Schedules => Set<Schedule>();
  public DbSet<WorkEvent> Events => Set<WorkEvent>();
  public DbSet<Vacation> VacationPeriods => Set<Vacation>();
  public DbSet<Role> Roles => Set<Role>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<RolePermission>().HasKey(r => new { r.RoleId, r.Type });
    modelBuilder
      .Entity<AssociatePermission>()
      .HasKey(p => new { p.AssociateId, p.Type });

    modelBuilder.Entity<SharedSchedule>();
    modelBuilder.Entity<AssociateSchedule>();
  }
}
