using Application.DTOs.SsomaAssignmanetType;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.SsomaAsiggnmanetType
{
    public class SsomaAsiggnmanetTypeUpdateValidator : AbstractValidator<SsomaAsiggnmanetTypeUpdateDto>
    {
        public SsomaAsiggnmanetTypeUpdateValidator()
        {
            RuleFor(x => x.SsomaAssignamentTypeId)
                    .GreaterThan(0).WithMessage("ID inválido")
                    .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.SsomaAssignamentName)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("El tipo de asignamiento obligatorio.")
                 .WithErrorCode(ErrorCodes.ValidationEmpty)

                 .MaximumLength(250)
                 .WithMessage("El tipo de asignamiento no debe exceder de los 250 caracteres.")
                 .WithErrorCode(ErrorCodes.ValidationLength)

                 .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                 .WithMessage("El tipo de asignamiento tiene caracteres especiales.")
                 .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                 .Must(InputValidationHelpers.IsSafe)
                 .WithMessage("El tipo de asignamiento contiene caracteres peligrosos.")
                 .WithErrorCode(ErrorCodes.ValidationIllegalChar);
        }
    }
}
