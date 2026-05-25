using Application.DTOs.Users;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Users
{
    public class UsersSettingUpdateValidator : AbstractValidator<UsersSettingUpdateDto>
    {
        public UsersSettingUpdateValidator() {

            RuleFor(x => x.UsersId)
            .GreaterThan(0).WithMessage("ID inválido")
            .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.UsersName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("El nombre de usuario es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(450)
                    .WithMessage("El nombre de usuario no debe exceder los 450 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)
               
                .Must(InputValidationHelpers.IsSafe)
                 .WithMessage("El nombre contiene caracteres peligrosos.")
                 .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.UsersLastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("El apellido de usuario es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(450)
                    .WithMessage("El apellido de usuario no debe exceder los 450 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)
                

                .Must(InputValidationHelpers.IsSafe)
                 .WithMessage("El apellido contiene caracteres peligrosos.")
                 .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.UsersDocument)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
                   .WithMessage("El documento de usuario es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty)
               .MaximumLength(20)
                   .WithMessage("El documento de usuario no debe exceder los 20 caracteres.")
                   .WithErrorCode(ErrorCodes.ValidationLength)

               .Matches("^[a-zA-Z0-9_\\-]+$")
                   .WithMessage("El documento de usuario solo puede contener letras, números, guiones y guiones bajos.")
                   .WithErrorCode(ErrorCodes.ValidationUsernameUnsafe)

               .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El documento de usuario contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);


            RuleFor(x => x.UsersEmail)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("El correo electrónico es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationEmpty)

                .EmailAddress()
                    .WithMessage("El correo electrónico no es válido.")
                    .WithErrorCode(ErrorCodes.ValidationEmailInvalid)

                .MaximumLength(150)
                     .WithMessage("El correo electronico no debe exceder los 150 caracter.")
                     .WithErrorCode(ErrorCodes.ValidationLength)

                .Must(InputValidationHelpers.IsSafe)
                     .WithMessage("El correo contiene caracteres peligrosos.")
                     .WithErrorCode(ErrorCodes.ValidationIllegalChar);

           

            RuleFor(x => x.UsersBy)
            .GreaterThan(0)
            .WithMessage("El campo UsersBy debe ser mayor que 0.")
            .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
