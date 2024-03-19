using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct RoleDTO(
  Guid id,
  string name,
  List<PermissionType> permissions
) : IMappableTo<Role>
{
  public Role Map()
  {
    var id = this.id;
    return new(name)
    {
      Id = id,
      Permissions = permissions.ConvertAll((p) => new RolePermission(id, p))
    };
  }
}
