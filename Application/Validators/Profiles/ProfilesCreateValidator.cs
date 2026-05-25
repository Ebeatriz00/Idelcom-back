using Application.DTOs.Profiles;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Profiles
{
    public class ProfilesCreateValidator : AbstractValidator<ProfilesCreateDto>
    {
        public ProfilesCreateValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del perfil es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(250).WithMessage("El nombre no debe exceder los 250 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("El nombre tiene caracteres especiales.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El nombre contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(250).WithMessage("La descripción no debe exceder los 250 caracteres.")
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
