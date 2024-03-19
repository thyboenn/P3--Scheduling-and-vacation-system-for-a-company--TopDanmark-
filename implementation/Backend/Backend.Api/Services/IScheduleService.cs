using Backend.Api.Messages;

namespace Backend.Api.Services;

public interface IScheduleService
{
  public WorkEventDTO? CreateSharedEvent(
    Guid creatorAssociateId,
    CreateWorkEventDTO createEvent
  );
  public WorkEventDTO? CreateAssociateEvent(
    Guid creatorAssociateId,
    CreateAssociateEventDTO createAssociateEvent
  );
  public List<WorkEventDTO>? GetEvents(Guid accessorId, GetRangeDTO request);
  public bool DeleteEvent(Guid deleterAssociateId, Guid eventId);
  public bool EditEvent(Guid editorId, WorkEventDTO request);
}
