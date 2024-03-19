using Backend.Api.Messages;
using Backend.Api.Models.Schedules;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Models;

[Index(nameof(Email), IsUnique = true)]
public class Associate : IId, IMappableTo<AssociateDTO>, IMappableTo<MeDTO>
{
  public Associate(string name, string email)
  {
    Name = name;
    Email = email;
  }

  public Guid Id { get; set; }
  public string Name { get; set; }
  public string Email { get; set; }
  public int? MaxWorkHours { get; set; }
  public string? Color { get; set; }

  public Guid? RoleId { get; set; }
  public Role? Role { get; set; }
  public List<AssociatePermission> ExtraPermissions { get; set; } =
    new List<AssociatePermission>();
  public AssociateSchedule? AssociateSchedule { get; set; }

  public byte[]? PasswordHash { get; set; }

  public byte[]? PasswordSalt { get; set; }

  public bool HasAnyPermission(params PermissionType[] permissionTypes)
  {
    var rolePermissions =
      Role?.Permissions?.Select(p => p.Map()).ToList()
      ?? new List<PermissionType>();
    var extraPermissions = ExtraPermissions.ConvertAll(p => p.Map());

    var isAdmin = extraPermissions.Contains(PermissionType.Admin);
    var hasRequiredPermission = permissionTypes.Any(
      permissionType =>
        extraPermissions.Contains(permissionType)
        || rolePermissions.Contains(permissionType)
    );

    return hasRequiredPermission || isAdmin;
  }

  public AssociateDTO Map() =>
    new(
      id: Id,
      name: Name,
      email: Email,
      maxWorkHours: MaxWorkHours,
      extraPermissions: ExtraPermissions.ConvertAll(p => p.Map()),
      roleId: Role?.Id,
      color: Color,
      associateScheduleId: AssociateSchedule?.Id
    );

  MeDTO IMappableTo<MeDTO>.Map() =>
    new(
      id: Id,
      name: Name,
      email: Email,
      maxWorkHours: MaxWorkHours,
      permissions: ExtraPermissions
        .ConvertAll(p => p.Map())
        .Concat(
          Role?.Permissions.ConvertAll(p => p.Map()) ?? new List<PermissionType>()
        )
        .ToList(),
      roleId: Role?.Id,
      color: Color,
      associateScheduleId: AssociateSchedule?.Id
    );
}
