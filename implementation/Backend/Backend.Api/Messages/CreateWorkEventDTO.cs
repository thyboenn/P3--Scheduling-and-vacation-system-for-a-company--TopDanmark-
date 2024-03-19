using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct CreateWorkEventDTO(
  DateTime start,
  DateTime end,
  EventType eventType,
  bool workingFromHome,
  bool atThePhone,
  string? note
) : IMappableTo<WorkEvent>
{
  public WorkEvent Map() =>
    new(
      start: start,
      end: end,
      eventType: eventType,
      note: note,
      workingFromHome: workingFromHome,
      atThePhone: atThePhone
    );
}
