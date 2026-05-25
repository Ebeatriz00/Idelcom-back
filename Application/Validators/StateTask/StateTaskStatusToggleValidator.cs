using Application.DTOs.StateTask;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.StateTask
{
    public class StateTaskStatusToggleValidator : AbstractValidator<StateTaskStatusToggleDto>
    {
        public StateTaskStatusToggleValidator()
        {
            RuleFor(x => x.StateTaskId)
                .GreaterThan(0).WithMessage("ID inválido")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.Status)
                .Must(status => status == "1" || status == "0")
                .WithMessage("Estado inválido. Solo se permite '1' (Activo) o '0' (Inactivo)")
                .WithErrorCode(ErrorCodes.Conflict)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("El estado tiene caracteres especiales.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El estado contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);


            RuleFor(x => x.UsersBy)
                .GreaterThan(0)
                .WithMessage("El campo UsersBy debe ser mayor que 0.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
