namespace Backend.Api.Models.Schedules;

public class AssociateSchedule : Schedule
{
  public AssociateSchedule(Guid associateId)
  {
    AssociateId = associateId;
  }

  public Guid AssociateId { get; set; }
}
