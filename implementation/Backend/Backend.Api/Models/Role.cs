using Backend.Api.Messages;

namespace Backend.Api.Models;

public class Role : IId, IMappableTo<RoleDTO>
{
  public Role(string name)
  {
    Name = name;
  }

  public Guid Id { get; set; }
  public string Name { get; set; }

  public List<Associate> Associates { get; set; } = new List<Associate>();
  public List<RolePermission> Permissions { get; set; } =
    new List<RolePermission>();

  public RoleDTO Map() =>
    new(id: Id, name: Name, permissions: Permissions.ConvertAll((p) => p.Map()));
}
