using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct MeDTO(
  Guid id,
  string name,
  string email,
  int? maxWorkHours,
  List<PermissionType> permissions,
  Guid? roleId,
  string? color,
  Guid? associateScheduleId
);
