using Application.DTOs.StatePreSale;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.StatePreSale
{
    public class StatePreSaleUpdateValidator : AbstractValidator<StatePreSaleUpdateDto>
    {
        public StatePreSaleUpdateValidator()
        {
            RuleFor(x => x.StatePreSaleId)
                    .GreaterThan(0).WithMessage("El ID del estado de preventa es inválido.")
                    .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.StateColor)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("El color es obligatorio.")
                 .WithErrorCode(ErrorCodes.ValidationEmpty)

                 .MaximumLength(50) 
                 .WithMessage("El color no debe exceder de los 50 caracteres.")
                 .WithErrorCode(ErrorCodes.ValidationLength)

                 .Matches(@"^#?([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$").When(x => !string.IsNullOrWhiteSpace(x.StateColor))
                 .WithMessage("El color debe ser un código de color hexadecimal válido.")
                 .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                 .Must(InputValidationHelpers.IsSafe)
                 .WithMessage("El color contiene caracteres peligrosos.")
                 .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.StateDesc)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(100) 
                .WithMessage("La descripción no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción tiene caracteres especiales.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("La descripción contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.NumPercPro)
                .InclusiveBetween(0, 100).WithMessage("El porcentaje de probabilidad debe estar entre 0 y 100.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.NumOrder)
                .GreaterThan(0).WithMessage("El orden es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
