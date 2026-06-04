using Application.DTOs.MobileAppVersion;
using Application.UseCases.MobileAppVersion;
using Application.Validators.MobileAppVersion;
using Core.Entities.MobileAppVersion;
using Core.Interfaces.MobileAppVersion;
using FluentAssertions;
using Moq;
using Xunit;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Tests;

public class UpsertMobileAppVersionConfigTests
{
    [Fact]
    public async Task ExecuteAsync_WhenPayloadIsValid_UpsertsNormalizedConfig()
    {
        var repository = new Mock<IMobileAppVersionConfigRepository>();
        repository
            .Setup(x => x.UpsertActiveAsync(It.IsAny<MobileAppVersionConfig>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MobileAppVersionConfig config, CancellationToken _) =>
            {
                config.Id = 10;
                return config;
            });

        var useCase = CreateUseCase(repository.Object);

        var result = await useCase.ExecuteAsync(new MobileAppVersionUpsertDto
        {
            Platform = " ANDROID ",
            Environment = " PROD ",
            MinimumVersionName = " 1.4.2 ",
            MinimumBuildNumber = 9,
            LatestVersionName = " 1.4.2 ",
            LatestBuildNumber = 9,
            ForceUpdate = true,
            StoreUrl = " https://play.google.com/store/apps/details?id=TU_PACKAGE_NAME ",
            Message = " Hay una nueva version obligatoria. "
        });

        result.Status.Should().Be(1);
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(10);
        result.Data.Platform.Should().Be("android");
        result.Data.Environment.Should().Be("prod");
        result.Data.MinimumVersion.Should().Be("1.4.2+9");
        result.Data.LatestVersion.Should().Be("1.4.2+9");
        result.Data.AndroidUrl.Should().Be("https://play.google.com/store/apps/details?id=TU_PACKAGE_NAME");

        repository.Verify(x => x.UpsertActiveAsync(
            It.Is<MobileAppVersionConfig>(config =>
                config.Platform == "android" &&
                config.Environment == "prod" &&
                config.MinimumVersionName == "1.4.2" &&
                config.LatestVersionName == "1.4.2" &&
                config.StoreUrl == "https://play.google.com/store/apps/details?id=TU_PACKAGE_NAME"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenForceUpdateIsTrueAndStoreUrlIsEmpty_ThrowsValidationException()
    {
        var repository = new Mock<IMobileAppVersionConfigRepository>();
        var useCase = CreateUseCase(repository.Object);

        var act = async () => await useCase.ExecuteAsync(CreateValidDto(withStoreUrl: false));

        await act.Should().ThrowAsync<AppValidationException>();
        repository.Verify(x => x.UpsertActiveAsync(
                It.IsAny<MobileAppVersionConfig>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMinimumBuildIsGreaterThanLatestBuild_ThrowsValidationException()
    {
        var repository = new Mock<IMobileAppVersionConfigRepository>();
        var useCase = CreateUseCase(repository.Object);
        var dto = CreateValidDto();
        dto.MinimumBuildNumber = 10;
        dto.LatestBuildNumber = 9;

        var act = async () => await useCase.ExecuteAsync(dto);

        await act.Should().ThrowAsync<AppValidationException>();
        repository.Verify(x => x.UpsertActiveAsync(
                It.IsAny<MobileAppVersionConfig>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static UpsertMobileAppVersionConfig CreateUseCase(IMobileAppVersionConfigRepository repository)
        => new(repository, new MobileAppVersionUpsertValidator());

    private static MobileAppVersionUpsertDto CreateValidDto(bool withStoreUrl = true)
    {
        return new MobileAppVersionUpsertDto
        {
            Platform = "android",
            Environment = "prod",
            MinimumVersionName = "1.4.2",
            MinimumBuildNumber = 9,
            LatestVersionName = "1.4.2",
            LatestBuildNumber = 9,
            ForceUpdate = true,
            StoreUrl = withStoreUrl ? "https://play.google.com/store/apps/details?id=TU_PACKAGE_NAME" : null,
            Message = "Hay una nueva version obligatoria. Actualiza para continuar."
        };
    }
}
