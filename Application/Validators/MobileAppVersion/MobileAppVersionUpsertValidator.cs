using Application.DTOs.MobileAppVersion;
using FluentValidation;

namespace Application.Validators.MobileAppVersion
{
    public class MobileAppVersionUpsertValidator : AbstractValidator<MobileAppVersionUpsertDto>
    {
        private static readonly string[] SupportedPlatforms = ["android"];
        private static readonly string[] SupportedEnvironments = ["prod", "dev", "staging"];

        public MobileAppVersionUpsertValidator()
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

            RuleFor(x => x.MinimumVersionName)
                .NotEmpty()
                .WithErrorCode("MOBILE_APP_VERSION_MINIMUM_VERSION_REQUIRED")
                .WithMessage("La version minima es requerida.")
                .MaximumLength(30)
                .WithErrorCode("MOBILE_APP_VERSION_MINIMUM_VERSION_TOO_LONG")
                .WithMessage("La version minima no debe superar 30 caracteres.");

            RuleFor(x => x.LatestVersionName)
                .NotEmpty()
                .WithErrorCode("MOBILE_APP_VERSION_LATEST_VERSION_REQUIRED")
                .WithMessage("La ultima version es requerida.")
                .MaximumLength(30)
                .WithErrorCode("MOBILE_APP_VERSION_LATEST_VERSION_TOO_LONG")
                .WithMessage("La ultima version no debe superar 30 caracteres.");

            RuleFor(x => x.MinimumBuildNumber)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode("MOBILE_APP_VERSION_MINIMUM_BUILD_INVALID")
                .WithMessage("El build minimo debe ser mayor o igual a cero.");

            RuleFor(x => x.LatestBuildNumber)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode("MOBILE_APP_VERSION_LATEST_BUILD_INVALID")
                .WithMessage("El ultimo build debe ser mayor o igual a cero.")
                .GreaterThanOrEqualTo(x => x.MinimumBuildNumber)
                .WithErrorCode("MOBILE_APP_VERSION_BUILD_ORDER_INVALID")
                .WithMessage("El ultimo build no puede ser menor que el build minimo.");

            RuleFor(x => x.StoreUrl)
                .NotEmpty()
                .When(x => x.ForceUpdate)
                .WithErrorCode("MOBILE_APP_VERSION_STORE_URL_REQUIRED")
                .WithMessage("La URL de tienda es requerida cuando la actualizacion es obligatoria.")
                .MaximumLength(500)
                .WithErrorCode("MOBILE_APP_VERSION_STORE_URL_TOO_LONG")
                .WithMessage("La URL de tienda no debe superar 500 caracteres.");

            RuleFor(x => x.Message)
                .MaximumLength(500)
                .WithErrorCode("MOBILE_APP_VERSION_MESSAGE_TOO_LONG")
                .WithMessage("El mensaje no debe superar 500 caracteres.");
        }

        private static string Normalize(string? value) => value?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
