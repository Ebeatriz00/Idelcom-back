using Application.DTOs.Modules;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Modules
{
    public class ModulesUpdateValidator : AbstractValidator<ModulesUpdateDto>
    {
        public ModulesUpdateValidator() {

            RuleFor(x => x.ModulesId)
                .GreaterThan(0).WithMessage("ID inválido")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.Label)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del perfil es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50).WithMessage("El nombre no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("El nombre tiene caracteres especiales.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El nombre contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.ModulesDescription)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(150).WithMessage("La descripción no debe exceder los 100 caracteres.")
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
