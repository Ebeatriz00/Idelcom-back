using Application.DTOs.ModulePermission;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ModulePermission
{
    
        public class ModulesPermissionsUpdateValidator : AbstractValidator<ModulesPermissionsUpdateDto>
        {
            public ModulesPermissionsUpdateValidator()
            {
                RuleFor(x => x.ModulesPermissionsId)
                    .GreaterThan(0).WithMessage("ID inválido")
                    .WithErrorCode(ErrorCodes.Conflict);

                RuleFor(x => x.BusinessId)
                    .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

                RuleFor(x => x.ModulesId)
                    .GreaterThan(0).WithMessage("El módulo es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

                RuleFor(x => x.PermissionsId)
                    .GreaterThan(0).WithMessage("El permiso es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

                RuleFor(x => x.UsersBy)
                    .GreaterThan(0).WithMessage("El usuario es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            }
        }
}
