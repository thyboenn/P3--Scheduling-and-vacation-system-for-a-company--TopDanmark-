using Backend.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Repositories;

public class AssociateRepository : Repository<Associate>, IAssociateRepository
{
  public AssociateRepository(ApplicationDbContext context) : base(context) { }

  public override Associate? Read(Guid id) =>
    DbSet
      .Include(associate => associate.ExtraPermissions)
      .Include(associate => associate.AssociateSchedule)
      .Include(associate => associate.Role)
      .ThenInclude(role => role!.Permissions)
      .SingleOrDefault(associate => associate.Id == id);

  public override List<Associate> ReadAll() =>
    DbSet
      .Include(associate => associate.AssociateSchedule)
      .Include(associate => associate.ExtraPermissions)
      .Include(associate => associate.Role)
      .ToList();

  public Associate? ReadByEmail(string email) =>
    DbSet.FirstOrDefault(a => a.Email == email);
}
