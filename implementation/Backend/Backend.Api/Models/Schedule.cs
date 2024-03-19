namespace Backend.Api.Models;

public abstract class Schedule : IId
{
  public Guid Id { get; set; }
  public List<WorkEvent> Events { get; set; } = new List<WorkEvent>();
  public List<Vacation> VacationPeriods { get; set; } = new List<Vacation>();
}
