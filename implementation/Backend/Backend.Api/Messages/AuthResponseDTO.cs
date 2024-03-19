namespace Backend.Api.Messages;

public readonly record struct AuthResponseDTO(
  string accessToken,
  string refreshToken
);
