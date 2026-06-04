using Application.DTOs.MobileAppVersion;
using Core.Entities.MobileAppVersion;
using Core.Interfaces.MobileAppVersion;
using FluentValidation;
using SharedKernel;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.MobileAppVersion
{
    public class UpsertMobileAppVersionConfig
    {
        private readonly IMobileAppVersionConfigRepository _repository;
        private readonly IValidator<MobileAppVersionUpsertDto> _validator;

        public UpsertMobileAppVersionConfig(
            IMobileAppVersionConfigRepository repository,
            IValidator<MobileAppVersionUpsertDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse<MobileAppVersionAdminResponseDto>> ExecuteAsync(
            MobileAppVersionUpsertDto dto,
            CancellationToken cancellationToken = default)
        {
            Normalize(dto);

            var validation = await _validator.ValidateAsync(dto, cancellationToken);

            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }

            var saved = await _repository.UpsertActiveAsync(new MobileAppVersionConfig
            {
                Platform = dto.Platform,
                Environment = dto.Environment,
                MinimumVersionName = dto.MinimumVersionName,
                MinimumBuildNumber = dto.MinimumBuildNumber,
                LatestVersionName = dto.LatestVersionName,
                LatestBuildNumber = dto.LatestBuildNumber,
                ForceUpdate = dto.ForceUpdate,
                StoreUrl = dto.StoreUrl,
                Message = dto.Message,
                IsActive = true
            }, cancellationToken);

            return new GlobalResponse<MobileAppVersionAdminResponseDto>
            {
                Status = 1,
                Message = "Configuracion de version movil actualizada correctamente.",
                Data = new MobileAppVersionAdminResponseDto
                {
                    Id = saved.Id,
                    Platform = saved.Platform,
                    Environment = saved.Environment,
                    MinimumVersion = FormatVersion(saved.MinimumVersionName, saved.MinimumBuildNumber),
                    MinimumBuildNumber = saved.MinimumBuildNumber,
                    LatestVersion = FormatVersion(saved.LatestVersionName, saved.LatestBuildNumber),
                    LatestBuildNumber = saved.LatestBuildNumber,
                    ForceUpdate = saved.ForceUpdate,
                    AndroidUrl = saved.Platform == "android" ? saved.StoreUrl : null,
                    Message = saved.Message
                }
            };
        }

        private static void Normalize(MobileAppVersionUpsertDto dto)
        {
            dto.Platform = Normalize(dto.Platform);
            dto.Environment = Normalize(dto.Environment);
            dto.MinimumVersionName = dto.MinimumVersionName.Trim();
            dto.LatestVersionName = dto.LatestVersionName.Trim();
            dto.StoreUrl = NormalizeNullable(dto.StoreUrl);
            dto.Message = NormalizeNullable(dto.Message);
        }

        private static string FormatVersion(string versionName, int buildNumber)
            => $"{versionName}+{buildNumber}";

        private static string Normalize(string? value) => value?.Trim().ToLowerInvariant() ?? string.Empty;

        private static string? NormalizeNullable(string? value)
        {
            var normalized = value?.Trim();
            return string.IsNullOrEmpty(normalized) ? null : normalized;
        }
    }
}
