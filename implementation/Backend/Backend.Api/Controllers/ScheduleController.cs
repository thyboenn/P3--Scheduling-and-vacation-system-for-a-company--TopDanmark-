using Backend.Api.Messages;
using Backend.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController : ControllerBase
{
  private readonly IScheduleService _scheduleService;

  public ScheduleController(IScheduleService scheduleService)
  {
    _scheduleService = scheduleService;
  }

  [HttpPost("shared")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkEventDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult CreateSharedEvent(CreateWorkEventDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _scheduleService.CreateSharedEvent(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpPost("associate")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkEventDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult CreateAssociateEvent(CreateAssociateEventDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _scheduleService.CreateAssociateEvent(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }

  // TODO: Should be get. Maybe use a query parameter
  [HttpPost("events")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WorkEventDTO>))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult GetEvents(GetRangeDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _scheduleService.GetEvents(userId.Value, request);

    return result == null ? BadRequest() : Ok(result);
  }

  [HttpPut("edit")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult EditEvent(WorkEventDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _scheduleService.EditEvent(userId.Value, request);

    return !result ? BadRequest() : Ok(result);
  }

  [HttpPost(template: "delete")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult DeleteEvent(DeleteEventDTO request)
  {
    var userId = this.GetUserIdFromToken();

    if (userId == null)
      return BadRequest();

    var result = _scheduleService.DeleteEvent(userId.Value, request.eventId);

    return !result ? BadRequest() : Ok(result);
  }
}
