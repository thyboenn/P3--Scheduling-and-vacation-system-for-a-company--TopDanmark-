using Backend.Api.Models;
using Backend.Api.Repositories;
using Backend.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Backend.Api.Test;

public class TestContainer
{
  public TestContainer()
  {
    TestingDbContext = new();

    ScheduleRepo = new(TestingDbContext);
    AssociateRepo = new(TestingDbContext);
    WorkEventRepo = new(TestingDbContext);
    VacationRepo = new(TestingDbContext);

    AssociateService = new(
      new Mock<ILogger<AssociateService>>().Object,
      AssociateRepo,
      ScheduleRepo
    );
    ScheduleService = new(ScheduleRepo, WorkEventRepo, AssociateRepo);
    VacationService = new(VacationRepo, AssociateRepo, ScheduleRepo);
  }

  public TestingDbContext TestingDbContext { get; set; }
  public Repository<Schedule> ScheduleRepo { get; set; }
  public AssociateRepository AssociateRepo { get; set; }
  public Repository<WorkEvent> WorkEventRepo { get; set; }
  public VacationRepository VacationRepo { get; set; }
  public AssociateService AssociateService { get; set; }
  public ScheduleService ScheduleService { get; set; }
  public VacationService VacationService { get; set; }
}
