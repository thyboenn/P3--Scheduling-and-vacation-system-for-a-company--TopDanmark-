using Backend.Api.Messages;

namespace Backend.Api.Services;

public interface IRoleService
{
  public RoleDTO? CreateRole(Guid creatorId, CreateRoleDTO request);
  public List<RoleDTO>? GetRoles(Guid readerId);
  public RoleDTO? EditRole(Guid editorId, RoleDTO role);
}
