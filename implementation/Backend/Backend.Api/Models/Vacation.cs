using Backend.Api.Messages;
using Backend.Api.Models.Schedules;

namespace Backend.Api.Models;

public class Vacation : IId, IMappableTo<VacationDTO>
{
  public Vacation(DateTime start, DateTime end, VacationStatus status)
  {
    Start = start;
    End = end;
    Status = status;
  }

  public Guid Id { get; set; }
  public DateTime Start { get; set; }
  public DateTime End { get; set; }
  public VacationStatus Status { get; set; }
  public bool IsSealed { get; set; }

  public Guid ScheduleId { get; set; }
  public Schedule? Schedule { get; set; }

  public VacationDTO Map() =>
    new(id: Id, start: Start, end: End, status: Status, scheduleId: ScheduleId);
}

public enum VacationStatus
{
  Approved,
  Pending,
  Denied,
}
