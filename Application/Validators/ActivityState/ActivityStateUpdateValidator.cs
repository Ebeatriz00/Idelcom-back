using Application.DTOs.ActivityState;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ActivityState
{
    public class ActivityStateUpdateValidator : AbstractValidator<ActivityStateUpdateDto>
    {

        public ActivityStateUpdateValidator() {

            RuleFor(x => x.LinkToken)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El ID es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.StateDesc)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción del área es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(300).WithMessage("La descripción no debe exceder los 300 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción tiene caracteres especiales no permitidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("La descripción contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

        }
    }
}
