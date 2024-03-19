using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct SetVacationStatusDTO(
  Guid vacationId,
  VacationStatus vacationStatus
);
