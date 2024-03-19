using Backend.Api.Messages;
using Xunit;
using System;
using FluentAssertions;
using Backend.Api.Models;

namespace Backend.Api.Test;

public class VacationServiceTests
{
  [Fact]
  public void RequestVacation()
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
    VacationDTO? result = tc.VacationService.RequestVacation(
      requesterId: adminAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );

    // Assert
    tc.VacationRepo.Read(result!.Value.id)!.Should().NotBeNull();
  }

  [Fact]
  public void DeleteVacation()
  {
    //  Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    VacationDTO? requestVacation1 = tc.VacationService.RequestVacation(
      requesterId: adminAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );
    requestVacation1.Should().NotBeNull();

    VacationDTO? requestVacation2 = tc.VacationService.RequestVacation(
      requesterId: adminAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );
    requestVacation2.Should().NotBeNull();

    VacationDTO? requestVacation3 = tc.VacationService.RequestVacation(
      requesterId: adminAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );
    requestVacation3.Should().NotBeNull();

    tc.VacationService.DeleteVacation(
      deleterId: adminAssociate.id,
      request: new(requestVacation1!.Value.id)
    );

    tc.VacationService.DeleteVacation(
      deleterId: regularAssociate.id,
      request: new(requestVacation2!.Value.id)
    );

    // Assert
    tc.VacationRepo.Count().Should().Be(2);
  }

  [Fact]
  public void EditVacation()
  {
    //  Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    VacationDTO? requestVacation = tc.VacationService.RequestVacation(
      requesterId: adminAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );

    VacationDTO? requestVacation1 = tc.VacationService.RequestVacation(
      requesterId: adminAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );

    requestVacation.Should().NotBeNull();
    requestVacation1.Should().NotBeNull();

    VacationDTO? editVacation1 = tc.VacationService.EditVacation(
      regularAssociate.id,
      request: new(
        id: requestVacation!.Value.id,
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 27)
      )
    );

    VacationDTO? editVacation = tc.VacationService.EditVacation(
      adminAssociate.id,
      request: new(
        id: requestVacation!.Value.id,
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 27)
      )
    );

    // Assert
    requestVacation.Should().NotBe(editVacation);
    editVacation1.Should().BeNull();
  }

  [Fact]
  public void UpdateVacationStatus()
  {
    //  Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    tc.AssociateService
      .AddPermission(
        adderAssociateId: adminAssociate.id,
        request: new TogglePermissionDTO(
          regularAssociate.id,
          PermissionType.CanEditOwnVacations
        )
      )
      .Should()
      .BeTrue();

    // Act
    VacationDTO? requestVacation1 = tc.VacationService.RequestVacation(
      requesterId: regularAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );
    requestVacation1.Should().NotBeNull();

    VacationDTO? requestVacation2 = tc.VacationService.RequestVacation(
      requesterId: regularAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );
    requestVacation2.Should().NotBeNull();

    VacationDTO? requestVacation3 = tc.VacationService.RequestVacation(
      requesterId: regularAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );
    requestVacation3.Should().NotBeNull();

    VacationDTO? updatedVacationToApproved = tc.VacationService.SetVacationStatus(
      setterId: adminAssociate.id,
      request: new(
        vacationId: requestVacation1!.Value.id,
        vacationStatus: VacationStatus.Approved
      )
    );

    VacationDTO? updatedVacationToDenied = tc.VacationService.SetVacationStatus(
      setterId: adminAssociate.id,
      request: new(
        vacationId: requestVacation2!.Value.id,
        vacationStatus: VacationStatus.Denied
      )
    );

    VacationDTO? regularUpdatedVacation = tc.VacationService.SetVacationStatus(
      setterId: regularAssociate.id,
      request: new(
        vacationId: requestVacation3!.Value.id,
        vacationStatus: VacationStatus.Approved
      )
    );

    // Assert
    updatedVacationToApproved!.Value.status.Should().Be(VacationStatus.Approved);
    updatedVacationToDenied!.Value.status.Should().Be(VacationStatus.Denied);
    regularUpdatedVacation.Should().BeNull();
    tc.VacationRepo
      .Read(requestVacation3!.Value.id)!
      .Status.Should()
      .Be(VacationStatus.Pending);
  }

  [Fact]
  public void SealVacation()
  {
    //  Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );

    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    VacationDTO? requestVacation = tc.VacationService.RequestVacation(
      requesterId: adminAssociate.id,
      request: new(
        start: new DateTime(2022, 10, 14),
        end: new DateTime(2022, 10, 25)
      )
    );
    requestVacation.Should().NotBeNull();

    bool updatedVacationSeal = tc.VacationService.SealVacation(
      sealerId: adminAssociate.id,
      request: new(requestVacation!.Value.id)
    );

    // Assert
    updatedVacationSeal.Should().BeTrue();
  }
}
