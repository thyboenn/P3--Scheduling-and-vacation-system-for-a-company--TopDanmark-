using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct WorkEventDTO(
  Guid id,
  DateTime start,
  DateTime end,
  EventType eventType,
  bool workingFromHome,
  bool atThePhone,
  Guid scheduleId,
  string? note
);
