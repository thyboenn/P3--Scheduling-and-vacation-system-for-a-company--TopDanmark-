using Backend.Api.Models;

namespace Backend.Api.Messages;

public readonly record struct CreateRoleDTO(string name) : IMappableTo<Role>
{
  public Role Map() => new(name: name);
}
