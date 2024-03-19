namespace Backend.Api.Messages;

public readonly record struct ChangeRoleDTO(Guid associateId, Guid roleId);
