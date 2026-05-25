using Application.DTOs.ActivityType;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ActivityType
{
    public class ActivityTypeCreateValidator : AbstractValidator<ActivityTypeCreateDto>
    {
        public ActivityTypeCreateValidator() {

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ActivityDesc)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de actividad es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(250).WithMessage("El tipo de actividad no debe exceder los 250 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("El tipo de actividad tiene caracteres especiales no permitidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El tipo de actividad contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

        }
    }
}
