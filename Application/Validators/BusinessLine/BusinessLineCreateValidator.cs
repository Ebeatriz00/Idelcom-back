using Application.DTOs.BusinessLine;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.BusinessLine
{
    public class BusinessLineCreateValidator : AbstractValidator<BusinessLineCreateDto>
    {

        public BusinessLineCreateValidator() {

            RuleFor(x => x.BusinessId)
                    .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.DescLine)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("La descripción es obligatorio.")
                 .WithErrorCode(ErrorCodes.ValidationEmpty)

                 .MaximumLength(30)
                 .WithMessage("La descripción no debe exceder de los 30 caracteres.")
                 .WithErrorCode(ErrorCodes.ValidationLength)

                 .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                 .WithMessage("La descripción tiene caracteres especiales.")
                 .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                 .Must(InputValidationHelpers.IsSafe)
                 .WithMessage("La descripción contiene caracteres peligrosos.")
                 .WithErrorCode(ErrorCodes.ValidationIllegalChar);

        }
    }
}
