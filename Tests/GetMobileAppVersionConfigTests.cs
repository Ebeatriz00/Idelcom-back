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

public class GetMobileAppVersionConfigTests
{
    [Fact]
    public async Task ExecuteAsync_WhenActiveAndroidProdConfigExists_ReturnsFormattedVersionConfig()
    {
        var repository = new Mock<IMobileAppVersionConfigRepository>();
        repository
            .Setup(x => x.GetActiveAsync("android", "prod", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MobileAppVersionConfig
            {
                Id = 1,
                Platform = "android",
                Environment = "prod",
                MinimumVersionName = "1.4.2",
                MinimumBuildNumber = 9,
                LatestVersionName = "1.4.2",
                LatestBuildNumber = 9,
                ForceUpdate = true,
                StoreUrl = "https://play.google.com/store/apps/details?id=TU_PACKAGE_NAME",
                Message = "Hay una nueva version obligatoria. Actualiza para continuar.",
                IsActive = true
            });

        var useCase = CreateUseCase(repository.Object);

        var result = await useCase.ExecuteAsync(new MobileAppVersionQueryDto
        {
            Platform = "ANDROID",
            Environment = "PROD"
        });

        result.MinimumVersion.Should().Be("1.4.2+9");
        result.MinimumBuildNumber.Should().Be(9);
        result.LatestVersion.Should().Be("1.4.2+9");
        result.LatestBuildNumber.Should().Be(9);
        result.ForceUpdate.Should().BeTrue();
        result.AndroidUrl.Should().Be("https://play.google.com/store/apps/details?id=TU_PACKAGE_NAME");
        result.Message.Should().Be("Hay una nueva version obligatoria. Actualiza para continuar.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenActiveConfigDoesNotExist_ReturnsSafeFallback()
    {
        var repository = new Mock<IMobileAppVersionConfigRepository>();
        repository
            .Setup(x => x.GetActiveAsync("android", "prod", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MobileAppVersionConfig?)null);

        var useCase = CreateUseCase(repository.Object);

        var result = await useCase.ExecuteAsync(new MobileAppVersionQueryDto
        {
            Platform = "android",
            Environment = "prod"
        });

        result.MinimumVersion.Should().Be("0.0.0+0");
        result.MinimumBuildNumber.Should().Be(0);
        result.LatestVersion.Should().Be("0.0.0+0");
        result.LatestBuildNumber.Should().Be(0);
        result.ForceUpdate.Should().BeFalse();
        result.AndroidUrl.Should().BeNull();
        result.Message.Should().Be("Configuracion de version no disponible.");
    }

    [Theory]
    [InlineData("", "prod")]
    [InlineData("ios", "prod")]
    [InlineData("android", "")]
    [InlineData("android", "qa")]
    public async Task ExecuteAsync_WhenQueryIsInvalid_ThrowsValidationException(string platform, string environment)
    {
        var repository = new Mock<IMobileAppVersionConfigRepository>();
        var useCase = CreateUseCase(repository.Object);

        var act = async () => await useCase.ExecuteAsync(new MobileAppVersionQueryDto
        {
            Platform = platform,
            Environment = environment
        });

        await act.Should().ThrowAsync<AppValidationException>();
        repository.Verify(
            x => x.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static GetMobileAppVersionConfig CreateUseCase(IMobileAppVersionConfigRepository repository)
        => new(repository, new MobileAppVersionQueryValidator());
}
