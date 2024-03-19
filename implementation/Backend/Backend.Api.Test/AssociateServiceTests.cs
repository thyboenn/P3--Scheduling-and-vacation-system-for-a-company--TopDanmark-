using Backend.Api.Models;
using Backend.Api.Messages;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Backend.Api.Test;

public class AssociateServiceTests
{
  [Fact]
  public void CreateFirstUsers()
  {
    // Arrange
    TestContainer tc = new();

    // Act
    AssociateDTO associate1 = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );
    AssociateDTO associate2 = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Assert
    associate1.extraPermissions.Should().Contain(PermissionType.Admin);
    associate2.extraPermissions.Should().NotContain(PermissionType.Admin);
    tc.AssociateRepo.Count().Should().Be(2);
    tc.ScheduleRepo.Count().Should().Be(2);
  }

  [Fact]
  public void RemoveOtherUser()
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
    tc.AssociateService
      .DeleteAssociate(
        deleterAssociateId: regularAssociate.id,
        associateId: adminAssociate.id
      )
      .Should()
      .BeFalse();

    tc.AssociateService.DeleteAssociate(
      deleterAssociateId: adminAssociate.id,
      associateId: regularAssociate.id
    );

    // Assert
    tc.AssociateRepo.Count().Should().Be(1);
    tc.ScheduleRepo.Count().Should().Be(1);
  }

  [Fact]
  public void RemoveSelfUser()
  {
    // Arrange
    TestContainer tc = new();

    tc.AssociateService.CreateAssociate(TestingData.CreateAssociateDto1);
    AssociateDTO associateNoPermissions = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act
    tc.AssociateService.DeleteAssociate(
      deleterAssociateId: associateNoPermissions.id,
      associateId: associateNoPermissions.id
    );

    // Assert
    tc.AssociateRepo.Count().Should().Be(1);
    tc.ScheduleRepo.Count().Should().Be(1);
  }

  [Fact]
  public void CreateDuplicateAssociate()
  {
    // Arrange
    TestContainer tc = new();

    // Act
    tc.AssociateService.CreateAssociate(TestingData.CreateAssociateDto1);
    Assert.Throws<DbUpdateException>(
      () => tc.AssociateService.CreateAssociate(TestingData.CreateAssociateDto1)
    );

    // Assert
    tc.AssociateRepo.Count().Should().Be(1);
  }

  [Fact]
  public void ManagePermissions()
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
    tc.AssociateService
      .AddPermission(
        adderAssociateId: regularAssociate.id,
        request: new TogglePermissionDTO(
          associateId: regularAssociate.id,
          permission: PermissionType.Admin
        )
      )
      .Should()
      .BeFalse();

    tc.AssociateService
      .AddPermission(
        adderAssociateId: adminAssociate.id,
        request: new TogglePermissionDTO(
          associateId: regularAssociate.id,
          permission: PermissionType.CanManageAssociates
        )
      )
      .Should()
      .BeTrue();

    tc.AssociateService
      .AddPermission(
        adderAssociateId: regularAssociate.id,
        request: new TogglePermissionDTO(
          associateId: regularAssociate.id,
          permission: PermissionType.CanManageOwnSchedule
        )
      )
      .Should()
      .BeTrue();

    tc.AssociateService
      .AddPermission(
        adderAssociateId: regularAssociate.id,
        request: new TogglePermissionDTO(
          associateId: regularAssociate.id,
          permission: PermissionType.CanManageOwnSchedule
        )
      )
      .Should()
      .BeFalse();

    tc.AssociateService
      .RemovePermission(
        removerAssociateId: regularAssociate.id,
        request: new TogglePermissionDTO(
          associateId: regularAssociate.id,
          permission: PermissionType.CanManageAssociates
        )
      )
      .Should()
      .BeTrue();

    tc.AssociateService
      .RemovePermission(
        removerAssociateId: regularAssociate.id,
        request: new TogglePermissionDTO(
          associateId: regularAssociate.id,
          permission: PermissionType.Admin
        )
      )
      .Should()
      .BeFalse();

    // Assert
    tc.AssociateRepo.Count().Should().Be(2);
    tc.ScheduleRepo.Count().Should().Be(2);

    adminAssociate = tc.AssociateRepo.Read(adminAssociate.id)!.Map();
    adminAssociate.extraPermissions.Should().Contain(PermissionType.Admin);

    regularAssociate = tc.AssociateRepo.Read(regularAssociate.id)!.Map();
    regularAssociate.extraPermissions
      .Should()
      .Contain(PermissionType.CanManageOwnSchedule)
      .And.NotContain(PermissionType.CanManageAssociates);
  }

  [Fact]
  public void GetAllAssociates()
  {
    // Arrange
    TestContainer tc = new();

    AssociateDTO adminAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto1
    );

    AssociateDTO regularAssociate = tc.AssociateService.CreateAssociate(
      TestingData.CreateAssociateDto2
    );

    // Act / Assert
    tc.AssociateService
      .GetAllAssociates(accessorAssociateId: adminAssociate.id)
      .Should()
      .HaveCount(2);

    tc.AssociateService
      .GetAllAssociates(accessorAssociateId: regularAssociate.id)
      .Should()
      .BeNull();

    tc.AssociateService.AddPermission(
      adderAssociateId: adminAssociate.id,
      request: new TogglePermissionDTO(
        associateId: regularAssociate.id,
        permission: PermissionType.CanManageAssociates
      )
    );

    tc.AssociateService
      .GetAllAssociates(accessorAssociateId: regularAssociate.id)
      .Should()
      .HaveCount(2);
  }
}
