using Backend.Api.Messages;
using Backend.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VacationController : ControllerBase
{
  private readonly IVacationService _vacationService;

  public VacationController(IVacationService vacationService)
  {
    _vacationService = vacationService;
  }

  [HttpPost("request")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VacationDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult RequestVacation(RequestVacationDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _vacationService.RequestVacation(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpPost("force")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VacationDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult ForceVacation(RequestVacationDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _vacationService.ForceVacation(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpDelete("delete")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult DeleteVacation(DeleteVacationDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    return !_vacationService.DeleteVacation(userId.Value, request)
      ? BadRequest()
      : Ok();
  }

  [HttpPost("set-status")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VacationDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult SetVacationStatus(SetVacationStatusDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _vacationService.SetVacationStatus(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpPost(template: "edit")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VacationDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult EditVacation(EditVacationDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _vacationService.EditVacation(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }

  // TODO: Should be a GET request
  [HttpPost(template: "get")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<VacationDTO>))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult GetVacations(GetRangeDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _vacationService.GetVacations(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpPut(template: "seal")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult SealVacation(SealVacationDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    return _vacationService.SealVacation(userId.Value, request)
      ? BadRequest()
      : Ok();
  }
}
