using Application.DTOs.MobileAppVersion;
using FluentValidation;

namespace Application.Validators.MobileAppVersion
{
    public class MobileAppVersionQueryValidator : AbstractValidator<MobileAppVersionQueryDto>
    {
        private static readonly string[] SupportedPlatforms = ["android"];
        private static readonly string[] SupportedEnvironments = ["prod", "dev", "staging"];

        public MobileAppVersionQueryValidator()
        {
            RuleFor(x => x.Platform)
                .NotEmpty()
                .WithErrorCode("MOBILE_APP_VERSION_PLATFORM_REQUIRED")
                .WithMessage("La plataforma es requerida.")
                .Must(value => SupportedPlatforms.Contains(Normalize(value)))
                .WithErrorCode("MOBILE_APP_VERSION_PLATFORM_UNSUPPORTED")
                .WithMessage("La plataforma no esta soportada.");

            RuleFor(x => x.Environment)
                .NotEmpty()
                .WithErrorCode("MOBILE_APP_VERSION_ENVIRONMENT_REQUIRED")
                .WithMessage("El ambiente es requerido.")
                .Must(value => SupportedEnvironments.Contains(Normalize(value)))
                .WithErrorCode("MOBILE_APP_VERSION_ENVIRONMENT_UNSUPPORTED")
                .WithMessage("El ambiente no esta soportado.");
        }

        private static string Normalize(string? value) => value?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
