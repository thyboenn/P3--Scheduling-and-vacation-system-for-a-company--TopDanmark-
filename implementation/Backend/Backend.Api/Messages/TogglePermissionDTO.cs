using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct TogglePermissionDTO(
  Guid associateId,
  PermissionType permission
);
