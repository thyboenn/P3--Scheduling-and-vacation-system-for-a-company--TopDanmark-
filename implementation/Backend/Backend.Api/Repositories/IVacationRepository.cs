using Backend.Api.Models;

namespace Backend.Api.Repositories;

public interface IVacationRepository : IRepository<Vacation>
{
  public List<Vacation> ReadRange(DateTime start, DateTime end);
}
