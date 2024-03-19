using Backend.Api.Models;

namespace Backend.Api.Repositories;

public interface IAssociateRepository : IRepository<Associate>
{
  public Associate? ReadByEmail(string email);
}
