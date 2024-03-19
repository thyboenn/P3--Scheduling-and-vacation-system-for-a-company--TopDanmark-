using Backend.Api.Messages;
using Backend.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  private static string FingerprintCookieName => "fingerprint";

  [AllowAnonymous]
  [HttpPost("login")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult Login(LoginDTO request)
  {
    var loginResult = _authService.Login(request);

    return HandleAuthResult(loginResult);
  }

  [AllowAnonymous]
  [HttpPost("register")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult Register(RegisterDTO request)
  {
    var registerResult = _authService.Register(request);

    return HandleAuthResult(registerResult);
  }

  [AllowAnonymous]
  [HttpPost("refresh")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult Refresh(RefreshDTO request)
  {
    var fingerprintCookie = Request.Cookies.FirstOrDefault(
      cookie => cookie.Key == FingerprintCookieName
    );

    var refreshResult = _authService.Refresh(request, fingerprintCookie.Value);

    return HandleAuthResult(refreshResult);
  }

  private IActionResult HandleAuthResult(
    (string fingerprint, AuthResponseDTO response)? authResult
  )
  {
    if (authResult == null)
    {
      return BadRequest();
    }

    var (fingerprint, response) = authResult.Value;

    AppendFingerprintCookie(fingerprint);
    return Ok(response);
  }

  private void AppendFingerprintCookie(string fingerprint)
  {
    Response.Cookies.Append(
      FingerprintCookieName,
      fingerprint,
      new CookieOptions()
      {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        Secure = true
      }
    );
  }
}
