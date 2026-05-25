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
    public class StatePreSaleStatusToggleValidator : AbstractValidator<StatePreSaleStatusToggleDto>
    {
        public StatePreSaleStatusToggleValidator()
        {
            RuleFor(x => x.StatePreSaleId)
                .GreaterThan(0).WithMessage("El ID del estado de preventa es inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El ID del negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Status)
                .Must(status => status == "1" || status == "0")
                .WithMessage("Estado inválido. Solo se permite '1' (Activo) o '0' (Inactivo).")
                .WithErrorCode(ErrorCodes.Conflict)

               
                .Matches(@"^[01]+$") 
                .WithMessage("El estado solo puede contener '1' o '0'.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El estado contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0)
                .WithMessage("El usuario que realiza la acción es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
