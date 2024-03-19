using Backend.Api.Models;

namespace Backend.Api.Repositories;

public interface IRepository<T> where T : class, IId
{
  public T Create(T t);
  public T? Read(Guid id);
  public List<T> ReadAll();
  public IQueryable<T> Query();
  public T? Update(T newT);
  public T? Delete(Guid id);

  public int Count();
}
