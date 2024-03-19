using Backend.Api.Messages;
using Backend.Api.Models;
using Backend.Api.Models.Schedules;
using Backend.Api.Repositories;

namespace Backend.Api.Services;

public class ScheduleService : IScheduleService
{
  private readonly IRepository<Schedule> _scheduleRepository;
  private readonly IRepository<WorkEvent> _eventRepository;
  private readonly IAssociateRepository _associateRepository;

  public ScheduleService(
    IRepository<Schedule> scheduleRepository,
    IRepository<WorkEvent> eventRepository,
    IAssociateRepository associateRepository
  )
  {
    _scheduleRepository = scheduleRepository;
    _eventRepository = eventRepository;
    _associateRepository = associateRepository;
  }

  public WorkEventDTO? CreateSharedEvent(
    Guid creatorAssociateId,
    CreateWorkEventDTO createEvent
  )
  {
    var creator = _associateRepository.Read(creatorAssociateId);

    if (creator?.HasAnyPermission(PermissionType.CanManageSharedSchedule) != true)
      return null;

    Schedule? sharedSchedule = _scheduleRepository
      .Query()
      .OfType<SharedSchedule>()
      .FirstOrDefault();

    sharedSchedule ??= _scheduleRepository.Create(new SharedSchedule());

    WorkEvent workEvent = createEvent.Map();

    sharedSchedule.Events.Add(workEvent);

    _scheduleRepository.Update(sharedSchedule);

    return workEvent.Map();
  }

  public WorkEventDTO? CreateAssociateEvent(
    Guid creatorAssociateId,
    CreateAssociateEventDTO createAssociateEvent
  )
  {
    var creator = _associateRepository.Read(creatorAssociateId);

    if (
      creator?.HasAnyPermission(PermissionType.CanManageOtherAssociateSchedule)
      != true
    )
    {
      return null;
    }

    var associateSchedule = _scheduleRepository
      .Query()
      .OfType<AssociateSchedule>()
      .FirstOrDefault(s => s.AssociateId == createAssociateEvent.associateId);

    if (associateSchedule is null)
      return null;

    var createEvent = createAssociateEvent.createEvent;
    WorkEvent workEvent = createEvent.Map();

    associateSchedule.Events.Add(workEvent);
    _scheduleRepository.Update(associateSchedule);

    return workEvent.Map();
  }

  public bool DeleteEvent(Guid deleterAssociateId, Guid eventId)
  {
    var @event = _eventRepository.Read(eventId);
    if (@event is null)
      return false;

    var schedule = _scheduleRepository.Read(@event.ScheduleId);

    if (schedule is SharedSchedule)
    {
      return DeleteSharedEvent(deleterAssociateId, eventId);
    }
    else if (schedule is AssociateSchedule associateSchedule)
    {
      return DeleteAssociateEvent(deleterAssociateId, eventId, associateSchedule);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  public List<WorkEventDTO>? GetEvents(Guid accessorId, GetRangeDTO request)
  {
    var accessor = _associateRepository.Read(accessorId);

    if (accessor?.HasAnyPermission(PermissionType.CanSeeSchedules) != true)
      return null;

    return _eventRepository
      .Query()
      .Where(e => e.Start <= request.end && e.Start >= request.start)
      .Select((e) => e.Map())
      .ToList();
  }

  public bool EditEvent(Guid editorId, WorkEventDTO request)
  {
    var accessor = _associateRepository.Read(editorId);

    if (accessor == null)
      return false;

    var existingEvent = _eventRepository
      .Query()
      .FirstOrDefault(e => e.Id == request.id);

    if (existingEvent == null)
      return false;

    var schedule = _scheduleRepository.Read(existingEvent.ScheduleId);

    if (schedule == null || !HasPermissionToEditEvent(accessor, schedule))
      return false;

    existingEvent.Start = request.start;
    existingEvent.End = request.end;
    existingEvent.EventType = request.eventType;
    existingEvent.Note = request.note;
    existingEvent.AtThePhone = request.atThePhone;
    existingEvent.WorkingFromHome = request.workingFromHome;

    _eventRepository.Update(existingEvent);

    return true;
  }

  private bool HasPermissionToEditEvent(Associate accessor, Schedule schedule)
  {
    if (schedule is SharedSchedule)
      return accessor.HasAnyPermission(PermissionType.CanManageSharedSchedule);

    if (schedule is AssociateSchedule associateSchedule)
    {
      return accessor.Id == associateSchedule.AssociateId
        ? accessor.HasAnyPermission(PermissionType.CanManageOwnSchedule)
        : accessor.HasAnyPermission(PermissionType.CanManageOtherAssociateSchedule);
    }

    throw new NotImplementedException();
  }

  private bool DeleteAssociateEvent(
    Guid deleterAssociateId,
    Guid eventId,
    AssociateSchedule associateSchedule
  )
  {
    var deleter = _associateRepository.Read(deleterAssociateId);

    if (
      deleter == null
      || (
        !deleter.HasAnyPermission(PermissionType.CanManageOtherAssociateSchedule)
        && associateSchedule.AssociateId != deleterAssociateId
      )
    )
    {
      return false;
    }

    _eventRepository.Delete(eventId);
    return true;
  }

  private bool DeleteSharedEvent(Guid deleterAssociateId, Guid eventId)
  {
    var deleter = _associateRepository.Read(deleterAssociateId);

    if (deleter?.HasAnyPermission(PermissionType.CanManageSharedSchedule) != true)
      return false;

    _eventRepository.Delete(eventId);

    return true;
  }
}
