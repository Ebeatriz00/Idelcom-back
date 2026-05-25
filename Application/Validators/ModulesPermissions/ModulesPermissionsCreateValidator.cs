using Application.DTOs.ModulePermission;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ModulePermission
{
    public class ModulesPermissionsCreateValidator : AbstractValidator<ModulesPermissionsCreateDto>
    {
        public ModulesPermissionsCreateValidator() {
            RuleFor(x => x.BusinessId)
               .GreaterThan(0).WithMessage("El negocio es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ModulesId)
               .GreaterThan(0).WithMessage("El modulo es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.PermissionsId)
               .GreaterThan(0).WithMessage("El permiso es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
