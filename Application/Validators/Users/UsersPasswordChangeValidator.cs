using Application.DTOs.Users;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Users
{
    public class UsersPasswordChangeValidator : AbstractValidator<UsersPasswordChangeDto>
    {
        public UsersPasswordChangeValidator() {

            RuleFor(x => x.BusinessId)
               .GreaterThan(0)
               .WithMessage("El negocio es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersId)
           .GreaterThan(0).WithMessage("ID inválido")
           .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.UsersPassword)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
                   .WithMessage("La contraseña es obligatoria.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty)
               .MinimumLength(8)
                   .WithMessage("La contraseña debe tener al menos 8 caracteres.")
                   .WithErrorCode(ErrorCodes.ValidationLength)
               .Matches(@"[A-Z]+")
                   .WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                   .WithErrorCode(ErrorCodes.ValidationPasswordNoUppercase)
               .Matches(@"[a-z]+")
                   .WithMessage("La contraseña debe contener al menos una letra minúscula.")
                   .WithErrorCode(ErrorCodes.ValidationPasswordNoLowercase)
               .Matches(@"[0-9]+")
                   .WithMessage("La contraseña debe contener al menos un número.")
                   .WithErrorCode(ErrorCodes.ValidationPasswordNoDigit)
               .Matches(@"[!@#$%^&*()_+\-]+")
                   .WithMessage("La contraseña debe contener al menos un carácter especial.")
                   .WithErrorCode(ErrorCodes.ValidationPasswordNoSymbol);

            RuleFor(x => x.UsersBy)
            .GreaterThan(0)
            .WithMessage("El campo UsersBy debe ser mayor que 0.")
            .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

        }
    }
}
