using Backend.Api.Messages;
using Backend.Api.Models;
using Backend.Api.Models.Schedules;
using Backend.Api.Repositories;

namespace Backend.Api.Services;

public class AssociateService : IAssociateService
{
  private readonly ILogger<AssociateService> _logger;
  private readonly IAssociateRepository _associateRepository;
  private readonly IRepository<Schedule> _scheduleRepository;

  public AssociateService(
    ILogger<AssociateService> logger,
    IAssociateRepository associateRepository,
    IRepository<Schedule> scheduleRepository
  )
  {
    _logger = logger;
    _scheduleRepository = scheduleRepository;
    _associateRepository = associateRepository;
  }

  public AssociateDTO CreateAssociate(CreateAssociateDTO createAssociateDto)
  {
    var associate = _associateRepository.Create(
      new Associate(name: createAssociateDto.name, email: createAssociateDto.email)
    );
    _scheduleRepository.Create(new AssociateSchedule(associate.Id));

    if (_associateRepository.Count() == 1)
    {
      associate.ExtraPermissions.Add(
        new(associateId: associate.Id, type: PermissionType.Admin)
      );
      _associateRepository.Update(associate);
    }

    return associate.Map();
  }

  public bool AddPermission(Guid adderAssociateId, TogglePermissionDTO request)
  {
    var adder = _associateRepository.Read(adderAssociateId);
    var associate = _associateRepository.Read(request.associateId);

    if (
      adder is null
      || associate is null
      || !adder.HasAnyPermission(PermissionType.CanManageAssociates)
      || associate.ExtraPermissions
        .Select(p => p.Map())
        .Contains(request.permission)
    )
    {
      return false;
    }

    associate.ExtraPermissions.Add(new(associate.Id, request.permission));

    _associateRepository.Update(associate);

    _logger.LogInformation(
      "Added permission for associate with id: {EmployeeId}",
      request.associateId
    );

    return true;
  }

  public bool RemovePermission(Guid removerAssociateId, TogglePermissionDTO request)
  {
    var remover = _associateRepository.Read(removerAssociateId);
    var associate = _associateRepository.Read(request.associateId);

    if (
      remover is null
      || associate is null
      || !remover.HasAnyPermission(PermissionType.CanManageAssociates)
    )
    {
      return false;
    }

    var removedAmount = associate.ExtraPermissions.RemoveAll(
      p => p.Type == request.permission
    );

    _associateRepository.Update(associate);

    return removedAmount > 0;
  }

  public bool DeleteAssociate(Guid deleterAssociateId, Guid associateId)
  {
    if (deleterAssociateId == associateId)
    {
      _associateRepository.Delete(associateId);
      return true;
    }

    var deleter = _associateRepository.Read(deleterAssociateId);

    if (deleter?.HasAnyPermission(PermissionType.CanManageAssociates) != true)
      return false;

    var deletedAssociate = _associateRepository.Delete(associateId);

    return deletedAssociate != null;
  }

  public List<AssociateDTO>? GetAllAssociates(Guid accessorAssociateId)
  {
    var accessor = _associateRepository.Read(accessorAssociateId);

    if (
      accessor?.HasAnyPermission(
        PermissionType.CanManageAssociates,
        PermissionType.CanManageOtherAssociateSchedule,
        PermissionType.CanSeeSchedules
      ) != true
    )
    {
      return null;
    }

    return _associateRepository.ReadAll().ConvertAll(a => a.Map());
  }

  public MeDTO? Me(Guid accessorAssociateId)
  {
    IMappableTo<MeDTO>? accessor = _associateRepository.Read(accessorAssociateId);

    return accessor?.Map();
  }

  public bool ChangeRole(Guid changerAssociateId, ChangeRoleDTO request)
  {
    var changer = _associateRepository.Read(changerAssociateId);
    var associate = _associateRepository.Read(request.associateId);

    if (
      associate == null
      || changer?.HasAnyPermission(PermissionType.CanManageAssociates) != true
    )
    {
      return false;
    }

    associate.RoleId = request.roleId;

    _associateRepository.Update(associate);

    return true;
  }

  public bool ChangeColor(Guid changerAssociateId, ChangeAssociateColorDTO request)
  {
    var changer = _associateRepository.Read(changerAssociateId);
    var associate = _associateRepository.Read(request.associateId);

    if (
      associate == null
      || changer?.HasAnyPermission(PermissionType.CanManageAssociates) != true
    )
    {
      return false;
    }

    associate.Color = request.Map();

    _associateRepository.Update(associate);

    return true;
  }
}
