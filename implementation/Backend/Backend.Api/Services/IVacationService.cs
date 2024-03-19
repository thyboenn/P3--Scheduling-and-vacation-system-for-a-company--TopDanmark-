using Backend.Api.Messages;

namespace Backend.Api.Services;

public interface IVacationService
{
  public VacationDTO? RequestVacation(Guid requesterId, RequestVacationDTO request);

  public VacationDTO? ForceVacation(Guid creatorId, RequestVacationDTO request);

  public bool DeleteVacation(Guid deleterId, DeleteVacationDTO request);

  public VacationDTO? SetVacationStatus(
    Guid setterId,
    SetVacationStatusDTO request
  );

  public VacationDTO? EditVacation(Guid editorId, EditVacationDTO request);

  public List<VacationDTO>? GetVacations(Guid accessorId, GetRangeDTO request);

  public bool SealVacation(Guid sealerId, SealVacationDTO request);
}
