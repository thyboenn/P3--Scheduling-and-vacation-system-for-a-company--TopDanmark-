using Backend.Api.Messages;
using Backend.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
  private readonly IRoleService _roleService;

  public RoleController(IRoleService roleService)
  {
    _roleService = roleService;
  }

  [HttpGet]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RoleDTO>))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult GetRoles()
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _roleService.GetRoles(userId.Value);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult CreateRole(CreateRoleDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _roleService.CreateRole(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpPut("edit")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult EditRole(RoleDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _roleService.EditRole(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }
}
