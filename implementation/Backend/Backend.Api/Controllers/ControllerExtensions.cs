using Backend.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

internal static class ControllerExtensions
{
  public static Guid? GetUserIdFromToken(this ControllerBase controller)
  {
    string? userIdString = controller.User.FindFirst(CustomClaims.UserId)?.Value;

    return userIdString != null ? new Guid(userIdString) : null;
  }
}
