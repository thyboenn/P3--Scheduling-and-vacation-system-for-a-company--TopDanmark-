using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct VacationDTO(
  Guid id,
  DateTime start,
  DateTime end,
  VacationStatus status,
  Guid scheduleId
) : IMappableTo<Vacation>
{
  public Vacation Map() =>
    new(start: start, end: end, status: status)
    {
      Id = id,
      ScheduleId = scheduleId
    };
}
