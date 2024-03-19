using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct RequestVacationDTO(DateTime start, DateTime end)
  : IMappableTo<Vacation>
{
  public Vacation Map() =>
    new(start: start, end: end, status: VacationStatus.Pending);
}
