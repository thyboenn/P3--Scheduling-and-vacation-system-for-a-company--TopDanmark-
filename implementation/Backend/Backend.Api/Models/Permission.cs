namespace Backend.Api.Models;

public class AssociatePermission : IMappableTo<PermissionType>
{
  public AssociatePermission(Guid associateId, PermissionType type)
  {
    AssociateId = associateId;
    Type = type;
  }

  public Guid AssociateId { get; set; }
  public PermissionType Type { get; set; }

  public PermissionType Map() => Type;
}

public class RolePermission : IMappableTo<PermissionType>
{
  public RolePermission(Guid roleId, PermissionType type)
  {
    RoleId = roleId;
    Type = type;
  }

  public Guid RoleId { get; set; }
  public PermissionType Type { get; set; }

  public PermissionType Map() => Type;
}

public enum PermissionType
{
  Admin,
  CanManageOtherAssociateSchedule,
  CanManageOwnSchedule,
  CanManageSharedSchedule,
  CanManageAssociates,
  CanSeeSchedules,
  CanEditOwnVacations,
  CanEditOthersVacations,
  CanSeeVacations,
  CanApproveOrDenyVacation,
  CanSealVacation,
  CanManageForcedVacation
}
