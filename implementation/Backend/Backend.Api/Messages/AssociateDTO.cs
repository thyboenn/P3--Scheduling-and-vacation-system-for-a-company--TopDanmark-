using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct AssociateDTO(
  Guid id,
  string name,
  string email,
  int? maxWorkHours,
  List<PermissionType> extraPermissions,
  Guid? roleId,
  string? color,
  Guid? associateScheduleId
);
