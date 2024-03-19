using Backend.Api.Messages;
using Backend.Api.Models;
using Backend.Api.Repositories;

namespace Backend.Api.Services;

public class RoleService : IRoleService
{
  private readonly IRoleRepository _roleRepository;
  private readonly IAssociateRepository _associateRepository;

  public RoleService(
    IRoleRepository roleRepository,
    IAssociateRepository associateRepository
  )
  {
    _roleRepository = roleRepository;
    _associateRepository = associateRepository;
  }

  public RoleDTO? CreateRole(Guid creatorId, CreateRoleDTO request)
  {
    var creator = _associateRepository.Read(creatorId);

    if (creator?.HasAnyPermission(PermissionType.CanManageAssociates) != true)
      return null;

    return _roleRepository.Create(request.Map()).Map();
  }

  public RoleDTO? EditRole(Guid editorId, RoleDTO role)
  {
    var editor = _associateRepository.Read(editorId);

    if (editor?.HasAnyPermission(PermissionType.CanManageAssociates) != true)
      return null;

    return _roleRepository.Update(role.Map())?.Map();
  }

  public List<RoleDTO>? GetRoles(Guid readerId)
  {
    var reader = _associateRepository.Read(readerId);

    if (reader?.HasAnyPermission(PermissionType.CanManageAssociates) != true)
      return null;

    var roles = _roleRepository.ReadAll();

    return roles.ConvertAll(r => r.Map());
  }
}
