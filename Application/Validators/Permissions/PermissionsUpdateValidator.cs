using Application.DTOs.Permissions;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Permissions
{
    public class PermissionsUpdateValidator : AbstractValidator<PermissionsUpdateDto>

    {
        public PermissionsUpdateValidator()
        {

            RuleFor(x => x.PermissionsId)
                .GreaterThan(0).WithMessage("Permiso inválido")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.PermissionsName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del permiso es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50).WithMessage("El nombre no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("El nombre tiene caracteres especiales.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El nombre contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.PermissionsDescription)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(150).WithMessage("La descripción no debe exceder los 150 caracteres.")
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
