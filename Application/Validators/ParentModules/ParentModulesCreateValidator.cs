using Application.DTOs.ParentModules;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ParentModules
{
    public class ParentModulesCreateValidator : AbstractValidator<ParentModulesCreateDto>
    {
        public ParentModulesCreateValidator() {
            RuleFor(x => x.BusinessId)
                    .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Title)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("El nombre del módulo padre es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationEmpty)

               .MaximumLength(60).WithMessage("El nombre no debe exceder los 60 caracteres.")
               .WithErrorCode(ErrorCodes.ValidationLength)

               .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
               .WithMessage("El nombre tiene caracteres especiales.")
               .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

               .Must(InputValidationHelpers.IsSafe)
               .WithMessage("El nombre contiene caracteres peligrosos.")
               .WithErrorCode(ErrorCodes.ValidationIllegalChar);
        }
    }
}
