using Backend.Api.Messages;
using Backend.Api.Models;
using Backend.Api.Models.Schedules;
using Backend.Api.Repositories;

namespace Backend.Api.Services;

public class VacationService : IVacationService
{
  private readonly IVacationRepository _vacationRepo;
  private readonly IAssociateRepository _associateRepo;
  private readonly IRepository<Schedule> _scheduleRepo;

  public VacationService(
    IVacationRepository vacationRepo,
    IAssociateRepository associateRepo,
    IRepository<Schedule> scheduleRepo
  )
  {
    _vacationRepo = vacationRepo;
    _associateRepo = associateRepo;
    _scheduleRepo = scheduleRepo;
  }

  public bool DeleteVacation(Guid deleterId, DeleteVacationDTO request)
  {
    var deleter = _associateRepo.Read(deleterId);
    var vacation = _vacationRepo.Read(request.vacationId);

    if (deleter == null || vacation == null)
      return false;

    if (!HasPermissionToDeleteVacation(deleter, vacation))
      return false;

    _vacationRepo.Delete(request.vacationId);

    return true;
  }

  public VacationDTO? EditVacation(Guid editorId, EditVacationDTO request)
  {
    if (request.start > request.end)
      return null;

    var editor = _associateRepo.Read(editorId);
    var vacation = _vacationRepo.Read(request.id);

    if (editor == null || vacation == null)
      return null;

    if (!HasPermissionToEditVacation(editor, vacation))
      return null;

    vacation.Start = request.start;
    vacation.End = request.end;
    vacation.Status = VacationStatus.Pending;
    return _vacationRepo.Update(vacation)?.Map();
  }

  public List<VacationDTO>? GetVacations(Guid accessorId, GetRangeDTO request)
  {
    if (request.start > request.end)
      return null;

    var accessor = _associateRepo.Read(accessorId);

    if (accessor?.HasAnyPermission(PermissionType.CanSeeSchedules) != true)
      return null;

    return _vacationRepo
      .ReadRange(start: request.start, end: request.end)
      .ConvertAll(v => v.Map());
  }

  public VacationDTO? RequestVacation(Guid requesterId, RequestVacationDTO request)
  {
    var requester = _associateRepo.Read(requesterId);

    if (
      requester?.AssociateSchedule?.Id == null
      || requester?.HasAnyPermission(PermissionType.CanEditOwnVacations) != true
    )
    {
      return null;
    }

    var vacation = request.Map();
    vacation.ScheduleId = requester.AssociateSchedule.Id;

    return _vacationRepo.Create(vacation).Map();
  }

  public VacationDTO? ForceVacation(Guid creatorId, RequestVacationDTO request)
  {
    var creator = _associateRepo.Read(creatorId);

    if (creator?.HasAnyPermission(PermissionType.CanManageForcedVacation) != true)
      return null;

    Schedule? sharedSchedule = _scheduleRepo
      .Query()
      .OfType<SharedSchedule>()
      .FirstOrDefault();
    sharedSchedule ??= _scheduleRepo.Create(new SharedSchedule());

    var vacation = request.Map();
    vacation.Status = VacationStatus.Approved;
    vacation.ScheduleId = sharedSchedule!.Id;

    return _vacationRepo.Create(vacation).Map();
  }

  public VacationDTO? SetVacationStatus(Guid setterId, SetVacationStatusDTO request)
  {
    var setter = _associateRepo.Read(setterId);
    var vacation = _vacationRepo.Read(request.vacationId);

    if (setter == null || vacation?.IsSealed != false)
    {
      return null;
    }

    if (
      new List<VacationStatus>
      {
        VacationStatus.Approved,
        VacationStatus.Denied
      }.Contains(request.vacationStatus)
      && !setter.HasAnyPermission(PermissionType.CanApproveOrDenyVacation)
    )
    {
      return null;
    }

    vacation.Status = request.vacationStatus;
    return _vacationRepo.Update(vacation)?.Map();
  }

  public bool SealVacation(Guid sealerId, SealVacationDTO request)
  {
    var sealer = _associateRepo.Read(sealerId);
    var vacation = _vacationRepo.Read(request.vacationId);

    if (sealer == null || vacation == null)
      return false;

    if (!sealer.HasAnyPermission(PermissionType.CanSealVacation))
      return false;

    vacation.IsSealed = true;
    _vacationRepo.Update(vacation);

    return true;
  }

  private bool HasPermissionToDeleteVacation(Associate deleter, Vacation vacation)
  {
    if (vacation.Schedule is SharedSchedule)
    {
      return deleter.HasAnyPermission(PermissionType.CanManageForcedVacation);
    }
    else if (vacation.Schedule is AssociateSchedule associateSchedule)
    {
      return deleter.Id == associateSchedule.AssociateId
        ? deleter.HasAnyPermission(PermissionType.CanEditOwnVacations)
        : deleter.HasAnyPermission(PermissionType.CanEditOthersVacations);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  private bool HasPermissionToEditVacation(Associate editor, Vacation vacation)
  {
    if (vacation.Schedule is SharedSchedule)
    {
      return editor.HasAnyPermission(PermissionType.CanManageForcedVacation);
    }
    else if (vacation.Schedule is AssociateSchedule associateSchedule)
    {
      return editor.Id == associateSchedule.AssociateId
        ? editor.HasAnyPermission(PermissionType.CanEditOwnVacations)
        : editor.HasAnyPermission(PermissionType.CanEditOthersVacations);
    }
    else
    {
      throw new NotImplementedException();
    }
  }
}
