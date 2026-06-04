using Application.DTOs.ProfilesPermissions;
using Application.Services.InterfacesServices;
using Application.UseCases.ProfilesPermissions;
using Application.Validators.ProfilesPermissions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Moq;
using Xunit;

namespace Tests;

public sealed class ProfilesPermissionsCacheInvalidationTests
{
    private static IMapper CreateMapper()
    {
        var mapper = new Mock<IMapper>(MockBehavior.Strict);
        mapper.Setup(x => x.Map<ProfilesPermissions>(It.IsAny<ProfilesPermissionsUpdateDto>()))
            .Returns((ProfilesPermissionsUpdateDto dto) => new ProfilesPermissions
            {
                ProfilesPermissionsId = dto.ProfilesPermissionsId,
                BusinessId = dto.BusinessId,
                ProfilesId = dto.ProfilesId,
                ModulesPermissionsId = dto.ModulesPermissionsId,
                UsersBy = dto.UsersBy
            });

        return mapper.Object;
    }

    [Fact]
    public async Task Create_invalidates_profile_permission_cache_after_insert()
    {
        var repo = new Mock<IProfilesPermissionsRepository>(MockBehavior.Strict);
        var auth = new Mock<IAuthPermissionService>(MockBehavior.Strict);
        var dto = new ProfilesPermissionsCreateDto
        {
            BusinessId = 20,
            ProfilesId = 10,
            ModulesPermissionsId = [30],
            UsersBy = 40
        };

        repo.Setup(x => x.ExistsAsync(10, 30, 20, null)).ReturnsAsync(false);
        repo.Setup(x => x.AddAsync(It.Is<IEnumerable<ProfilesPermissions>>(items =>
                items.Single().ProfilesId == 10 &&
                items.Single().BusinessId == 20 &&
                items.Single().ModulesPermissionsId == 30)))
            .Returns(Task.CompletedTask);
        auth.Setup(x => x.Invalidate(10, 20));

        var useCase = new CreateProfilesPermissions(
            repo.Object,
            new ProfilesPermissionsCreateValidator(),
            CreateMapper(),
            auth.Object);

        await useCase.ExecuteAsync(dto);

        auth.Verify(x => x.Invalidate(10, 20), Times.Once);
        repo.VerifyAll();
        auth.VerifyAll();
    }

    [Fact]
    public async Task Update_invalidates_profile_permission_cache_only_when_update_succeeds()
    {
        var repo = new Mock<IProfilesPermissionsRepository>(MockBehavior.Strict);
        var auth = new Mock<IAuthPermissionService>(MockBehavior.Strict);
        var dto = new ProfilesPermissionsUpdateDto
        {
            ProfilesPermissionsId = 99,
            BusinessId = 20,
            ProfilesId = 10,
            ModulesPermissionsId = 30,
            UsersBy = 40
        };

        repo.Setup(x => x.ExistsAsync(10, 30, 20, 99)).ReturnsAsync(false);
        repo.Setup(x => x.UpdateAsync(It.Is<ProfilesPermissions>(entity =>
                entity.ProfilesPermissionsId == 99 &&
                entity.ProfilesId == 10 &&
                entity.BusinessId == 20 &&
                entity.ModulesPermissionsId == 30)))
            .ReturnsAsync(true);
        auth.Setup(x => x.Invalidate(10, 20));

        var useCase = new UpdateProfilesPermissions(
            repo.Object,
            new ProfilesPermissionsUpdateValidator(),
            CreateMapper(),
            auth.Object);

        await useCase.ExecuteAsync(dto);

        auth.Verify(x => x.Invalidate(10, 20), Times.Once);
        repo.VerifyAll();
        auth.VerifyAll();
    }

    [Fact]
    public async Task Patch_status_invalidates_using_profile_id_from_current_record()
    {
        var repo = new Mock<IProfilesPermissionsRepository>(MockBehavior.Strict);
        var auth = new Mock<IAuthPermissionService>(MockBehavior.Strict);
        var dto = new ProfilesPermissionsStatusToggleDto
        {
            ProfilesPermissionsId = 99,
            BusinessId = 20,
            Status = "0",
            UsersBy = 40
        };

        repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync(new ProfilesPermissions
        {
            ProfilesPermissionsId = 99,
            BusinessId = 20,
            ProfilesId = 10,
            ModulesPermissionsId = 30
        });
        repo.Setup(x => x.PatchStatusAsync(99, "0", 40, 20)).ReturnsAsync(true);
        auth.Setup(x => x.Invalidate(10, 20));

        var useCase = new PatchProfilesPermissionsStatus(
            repo.Object,
            new ProfilesPermissionsStatusToggleValidator(),
            auth.Object);

        await useCase.ExecuteAsync(dto);

        auth.Verify(x => x.Invalidate(10, 20), Times.Once);
        repo.VerifyAll();
        auth.VerifyAll();
    }
}
