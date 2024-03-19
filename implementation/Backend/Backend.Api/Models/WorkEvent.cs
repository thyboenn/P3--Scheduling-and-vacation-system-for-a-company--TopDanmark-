using Backend.Api.Messages;

namespace Backend.Api.Models;

public class WorkEvent : IId, IMappableTo<WorkEventDTO>
{
  public WorkEvent(
    DateTime start,
    DateTime end,
    EventType eventType,
    string? note,
    bool workingFromHome,
    bool atThePhone
  )
  {
    Start = start;
    End = end;
    EventType = eventType;
    Note = note;
    WorkingFromHome = workingFromHome;
    AtThePhone = atThePhone;
  }

  public Guid Id { get; set; }
  public DateTime Start { get; set; }
  public DateTime End { get; set; }
  public EventType EventType { get; set; }
  public string? Note { get; set; }
  public bool WorkingFromHome { get; set; }
  public bool AtThePhone { get; set; }

  public Guid ScheduleId { get; set; }
  public Schedule? Schedule { get; set; }

  public WorkEventDTO Map() =>
    new(
      id: Id,
      start: Start,
      end: End,
      eventType: EventType,
      workingFromHome: WorkingFromHome,
      atThePhone: AtThePhone,
      scheduleId: ScheduleId,
      note: Note
    );
}

public enum EventType
{
  None,
  Meeting
}
