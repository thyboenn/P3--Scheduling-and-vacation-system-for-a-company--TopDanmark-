namespace Backend.Api.Models;

public interface IMappableTo<T>
{
  public T Map();
}
