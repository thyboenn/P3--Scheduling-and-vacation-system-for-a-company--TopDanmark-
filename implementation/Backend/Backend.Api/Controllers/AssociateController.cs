using Backend.Api.Messages;
using Backend.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AssociateController : ControllerBase
{
  private readonly IAssociateService _associateService;

  public AssociateController(IAssociateService associateService)
  {
    _associateService = associateService;
  }

  [HttpGet]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssociateDTO>))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult ReadAll()
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _associateService.GetAllAssociates(userId.Value);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpGet("me")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult Me()
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _associateService.Me(userId.Value);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpPost("permission/add")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult AddPermission(TogglePermissionDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    return !_associateService.AddPermission(userId.Value, request)
      ? BadRequest()
      : Ok();
  }

  [HttpDelete("permission/remove")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult RemovePermission(TogglePermissionDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    return !_associateService.RemovePermission(userId.Value, request)
      ? BadRequest()
      : Ok();
  }

  [HttpPost("role")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult ChangeRole(ChangeRoleDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    return !_associateService.ChangeRole(userId.Value, request)
      ? BadRequest()
      : Ok();
  }

  [HttpPost("color")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult ChangeColor(ChangeAssociateColorDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    return !_associateService.ChangeColor(userId.Value, request)
      ? BadRequest()
      : Ok();
  }
}
