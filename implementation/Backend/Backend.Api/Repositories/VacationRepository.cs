using Backend.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Repositories;

public class VacationRepository : Repository<Vacation>, IVacationRepository
{
  public VacationRepository(ApplicationDbContext context) : base(context) { }

  public override Vacation? Read(Guid id) =>
    DbSet.Include(v => v.Schedule).FirstOrDefault(v => v.Id == id);

  public List<Vacation> ReadRange(DateTime start, DateTime end) =>
    DbSet
      .Where(
        v =>
          (v.Start >= start && v.Start <= end) || (v.End >= start && v.End <= end)
      )
      .ToList();
}
