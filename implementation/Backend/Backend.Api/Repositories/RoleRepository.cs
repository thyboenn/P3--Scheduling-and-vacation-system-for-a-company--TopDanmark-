using Backend.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
  public RoleRepository(ApplicationDbContext context) : base(context) { }

  public override List<Role> ReadAll() =>
    DbSet.Include(role => role.Permissions).ToList();

  public override Role? Update(Role newT)
  {
    var existing = DbSet.Include(role => role.Permissions).FirstOrDefault();
    if (existing == null)
      return null;

    var entry = Context.Entry(existing);
    Context.RemoveRange(
      Context.Set<RolePermission>().Where(rp => rp.RoleId == newT.Id)
    );
    existing.Permissions = newT.Permissions;
    entry.CurrentValues.SetValues(newT);

    Context.SaveChanges();

    return entry.Entity;
  }
}
