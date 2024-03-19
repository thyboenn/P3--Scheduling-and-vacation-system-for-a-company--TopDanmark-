using Backend.Api.Messages;

namespace Backend.Api.Services;

public interface IAssociateService
{
  public List<AssociateDTO>? GetAllAssociates(Guid accessorAssociateId);
  public AssociateDTO CreateAssociate(CreateAssociateDTO createAssociateDto);
  public bool DeleteAssociate(Guid deleterAssociateId, Guid associateId);
  public bool AddPermission(Guid adderAssociateId, TogglePermissionDTO request);
  public bool RemovePermission(
    Guid removerAssociateId,
    TogglePermissionDTO request
  );
  public MeDTO? Me(Guid accessorAssociateId);
  public bool ChangeRole(Guid changerAssociateId, ChangeRoleDTO request);
  public bool ChangeColor(Guid changerAssociateId, ChangeAssociateColorDTO request);
}
