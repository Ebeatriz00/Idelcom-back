using Application.DTOs.MobileAppVersion;
using Core.Entities.MobileAppVersion;
using Core.Interfaces.MobileAppVersion;
using FluentValidation;
using SharedKernel;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.MobileAppVersion
{
    public class GetMobileAppVersionConfig
    {
        private const string FallbackVersionName = "0.0.0";
        private const int FallbackBuildNumber = 0;
        private const string FallbackMessage = "Configuracion de version no disponible.";

        private readonly IMobileAppVersionConfigRepository _repository;
        private readonly IValidator<MobileAppVersionQueryDto> _validator;

        public GetMobileAppVersionConfig(
            IMobileAppVersionConfigRepository repository,
            IValidator<MobileAppVersionQueryDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<MobileAppVersionResponseDto> ExecuteAsync(
            MobileAppVersionQueryDto query,
            CancellationToken cancellationToken = default)
        {
            query.Platform = Normalize(query.Platform);
            query.Environment = Normalize(query.Environment);

            var validation = await _validator.ValidateAsync(query, cancellationToken);

            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }

            var config = await _repository.GetActiveAsync(query.Platform, query.Environment, cancellationToken);
            return config is null ? CreateFallbackResponse() : CreateResponse(config);
        }

        private static MobileAppVersionResponseDto CreateResponse(MobileAppVersionConfig config)
        {
            return new MobileAppVersionResponseDto
            {
                MinimumVersion = FormatVersion(config.MinimumVersionName, config.MinimumBuildNumber),
                MinimumBuildNumber = config.MinimumBuildNumber,
                LatestVersion = FormatVersion(config.LatestVersionName, config.LatestBuildNumber),
                LatestBuildNumber = config.LatestBuildNumber,
                ForceUpdate = config.ForceUpdate,
                AndroidUrl = config.Platform == "android" ? config.StoreUrl : null,
                Message = config.Message
            };
        }

        private static MobileAppVersionResponseDto CreateFallbackResponse()
        {
            return new MobileAppVersionResponseDto
            {
                MinimumVersion = FormatVersion(FallbackVersionName, FallbackBuildNumber),
                MinimumBuildNumber = FallbackBuildNumber,
                LatestVersion = FormatVersion(FallbackVersionName, FallbackBuildNumber),
                LatestBuildNumber = FallbackBuildNumber,
                ForceUpdate = false,
                AndroidUrl = null,
                Message = FallbackMessage
            };
        }

        private static string FormatVersion(string versionName, int buildNumber)
            => $"{versionName}+{buildNumber}";

        private static string Normalize(string? value) => value?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
