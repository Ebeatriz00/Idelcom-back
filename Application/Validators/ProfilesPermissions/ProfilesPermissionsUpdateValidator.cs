using Application.DTOs.ProfilesPermissions;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ProfilesPermissions
{
    public class ProfilesPermissionsUpdateValidator : AbstractValidator<ProfilesPermissionsUpdateDto>
    {
        public ProfilesPermissionsUpdateValidator() {

            RuleFor(x => x.ModulesPermissionsId)
                        .GreaterThan(0).WithMessage("ID inválido")
                        .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ProfilesId)
                .GreaterThan(0).WithMessage("El perfil es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ModulesPermissionsId)
                .GreaterThan(0).WithMessage("El permiso del módulo es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
