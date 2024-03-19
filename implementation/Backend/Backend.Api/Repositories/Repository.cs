using Backend.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Repositories;

public class Repository<T> : IRepository<T> where T : class, IId
{
  public Repository(ApplicationDbContext context)
  {
    Context = context;
  }

  protected ApplicationDbContext Context { get; init; }

  protected DbSet<T> DbSet => Context.Set<T>();

  public virtual T Create(T t)
  {
    var result = DbSet.Add(t);
    Context.SaveChanges();

    return result.Entity;
  }

  public virtual T? Delete(Guid id)
  {
    var t = DbSet.FirstOrDefault(t => t.Id == id);
    if (t == null)
      return null;

    Context.Remove(t);
    Context.SaveChanges();

    return t;
  }

  public virtual T? Read(Guid id) => DbSet.FirstOrDefault(t => t.Id == id);

  public virtual List<T> ReadAll() => DbSet.ToList();

  public IQueryable<T> Query() => DbSet.AsQueryable();

  public virtual T? Update(T newT)
  {
    if (!DbSet.Any(t => t.Id == newT.Id))
      return null;

    var updatedT = DbSet.Update(newT).Entity;
    Context.SaveChanges();
    return updatedT;
  }

  public int Count() => DbSet.Count();
}
