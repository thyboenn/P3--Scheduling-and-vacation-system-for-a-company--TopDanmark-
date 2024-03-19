namespace Backend.Api.Messages;

public readonly record struct EditVacationDTO(
  Guid id,
  DateTime start,
  DateTime end
);
