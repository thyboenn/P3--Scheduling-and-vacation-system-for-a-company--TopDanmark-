namespace Backend.Api.Messages;

public readonly record struct RegisterDTO(
  string name,
  string email,
  string password
);
