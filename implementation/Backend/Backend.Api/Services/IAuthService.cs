using Backend.Api.Messages;

namespace Backend.Api.Services;

public interface IAuthService
{
  public (string fingerprint, AuthResponseDTO response)? Login(LoginDTO request);
  public (string fingerprint, AuthResponseDTO response)? Register(
    RegisterDTO request
  );
  public (string fingerprint, AuthResponseDTO response)? Refresh(
    RefreshDTO request,
    string fingerprint
  );
}
