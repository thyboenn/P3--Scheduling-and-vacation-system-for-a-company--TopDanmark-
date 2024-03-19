namespace Backend.Api.Messages;

public readonly record struct CreateAssociateEventDTO(
  Guid associateId,
  CreateWorkEventDTO createEvent
);
