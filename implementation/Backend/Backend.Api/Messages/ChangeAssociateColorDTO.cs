using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct ChangeAssociateColorDTO(
  Guid associateId,
  byte r,
  byte g,
  byte b
) : IMappableTo<string>
{
  public string Map() => $"#{r:x2}{g:x2}{b:x2}";
}
