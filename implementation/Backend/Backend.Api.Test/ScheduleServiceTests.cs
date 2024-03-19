using System;
using Backend.Api.Models;
using Backend.Api.Messages;
using FluentAssertions;
using Xunit;
using System.Linq;

namespace Backend.Api.Test;

public class ScheduleServiceTests
{
  [Fact]
  public void CreateSharedEvents()
  {
    // Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    tc.ScheduleService
      .CreateSharedEvent(adminAssociate.id, TestingData.CreateWorkEventDto1)
      .Should()
      .NotBe(null);
    tc.ScheduleService
      .CreateSharedEvent(regularAssociate.id, TestingData.CreateWorkEventDto2)
      .Should()
      .Be(null);
    tc.AssociateService
      .AddPermission(
        adderAssociateId: adminAssociate.id,
        request: new TogglePermissionDTO(
          regularAssociate.id,
          PermissionType.CanManageSharedSchedule
        )
      )
      .Should()
      .BeTrue();
    tc.ScheduleService
      .CreateSharedEvent(regularAssociate.id, TestingData.CreateWorkEventDto2)
      .Should()
      .NotBe(null);

    // Assert
    tc.WorkEventRepo.Count().Should().Be(2);
  }

  [Fact]
  public void CreateAssociateEvents()
  {
    // Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    tc.ScheduleService
      .CreateAssociateEvent(
        creatorAssociateId: regularAssociate.id,
        createAssociateEvent: new(
          associateId: regularAssociate.id,
          TestingData.CreateWorkEventDto1
        )
      )
      .Should()
      .Be(null);
    tc.ScheduleService.CreateAssociateEvent(
      creatorAssociateId: adminAssociate.id,
      createAssociateEvent: new(
        associateId: adminAssociate.id,
        TestingData.CreateWorkEventDto1
      )
    );
    tc.ScheduleService.CreateAssociateEvent(
      creatorAssociateId: adminAssociate.id,
      createAssociateEvent: new(
        associateId: regularAssociate.id,
        TestingData.CreateWorkEventDto1
      )
    );

    // Assert
    tc.WorkEventRepo.Count().Should().Be(2);
  }

  [Fact]
  public void DeleteAssociateEvents()
  {
    // Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    WorkEventDTO? createdEvent = tc.ScheduleService.CreateAssociateEvent(
      creatorAssociateId: adminAssociate.id,
      createAssociateEvent: new(
        associateId: adminAssociate.id,
        TestingData.CreateWorkEventDto1
      )
    );
    createdEvent.Should().NotBeNull();
    tc.ScheduleService.CreateAssociateEvent(
      creatorAssociateId: adminAssociate.id,
      createAssociateEvent: new(
        associateId: regularAssociate.id,
        TestingData.CreateWorkEventDto1
      )
    );

    tc.ScheduleService
      .DeleteEvent(
        deleterAssociateId: adminAssociate.id,
        eventId: createdEvent!.Value.id
      )
      .Should()
      .BeTrue();

    // Assert
    tc.WorkEventRepo.Count().Should().Be(1);
  }

  [Fact]
  public void DeleteSharedEvents()
  {
    // Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    WorkEventDTO? createdEvent = tc.ScheduleService.CreateSharedEvent(
      adminAssociate.id,
      TestingData.CreateWorkEventDto1
    );
    createdEvent.Should().NotBeNull();
    tc.ScheduleService
      .CreateSharedEvent(adminAssociate.id, TestingData.CreateWorkEventDto1)
      .Should()
      .NotBe(null);
    tc.ScheduleService
      .DeleteEvent(
        deleterAssociateId: adminAssociate.id,
        eventId: createdEvent!.Value.id
      )
      .Should()
      .BeTrue();

    // Assert
    tc.WorkEventRepo.Count().Should().Be(1);
  }

  [Fact]
  public void GetEvents()
  {
    // Arrange
    TestContainer tc = new();
    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    tc.ScheduleService.CreateAssociateEvent(
      creatorAssociateId: adminAssociate.id,
      createAssociateEvent: new(
        associateId: adminAssociate.id,
        TestingData.CreateWorkEventDto1
      )
    );
    tc.ScheduleService.CreateAssociateEvent(
      creatorAssociateId: adminAssociate.id,
      createAssociateEvent: new(
        associateId: regularAssociate.id,
        TestingData.CreateWorkEventDto1
      )
    );
    tc.ScheduleService
      .GetEvents(
        accessorId: regularAssociate.id,
        request: new(
          start: new DateTime(2019, 10, 10, hour: 8, 0, 0),
          end: new DateTime(2021, 10, 10, hour: 12, 0, 0)
        )
      )
      .Should()
      .BeNull();

    AndConstraint<FluentAssertions.Collections.GenericCollectionAssertions<WorkEventDTO>> getEventsList =
      tc.ScheduleService
        .GetEvents(
          accessorId: adminAssociate.id,
          request: new(
            start: new DateTime(2019, 10, 10, hour: 8, 0, 0),
            end: new DateTime(2021, 10, 10, hour: 12, 0, 0)
          )
        )
        .Should()
        .HaveCount(2);

    // Assert
    tc.WorkEventRepo.Count().Should().Be(2);
  }

  [Fact]
  public void EditEvent()
  {
    // Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    WorkEventDTO? createdEvent = tc.ScheduleService.CreateAssociateEvent(
      creatorAssociateId: adminAssociate.id,
      createAssociateEvent: new(
        associateId: adminAssociate.id,
        TestingData.CreateWorkEventDto1
      )
    );
    createdEvent.Should().NotBeNull();

    WorkEventDTO editedEvent =
      new(
        id: createdEvent!.Value.id,
        start: new DateTime(2018, 10, 10, hour: 16, 0, 0),
        end: new DateTime(2022, 10, 10, hour: 17, 0, 0),
        eventType: EventType.None,
        workingFromHome: false,
        atThePhone: false,
        scheduleId: createdEvent!.Value.scheduleId,
        note: "This is a note"
      );

    tc.ScheduleService
      .EditEvent(regularAssociate.id, editedEvent)
      .Should()
      .BeFalse();

    tc.ScheduleService.EditEvent(adminAssociate.id, editedEvent).Should().BeTrue();
    WorkEventDTO? editedEventDb = tc.WorkEventRepo
      .Query()
      .OfType<WorkEvent>()
      .FirstOrDefault(e => e.Id == editedEvent.id)
      ?.Map();

    // Assert
    editedEvent.Should().BeEquivalentTo(editedEventDb);
  }
}
