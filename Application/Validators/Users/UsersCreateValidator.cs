using Application.DTOs.Users;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;

namespace Application.Validators.Users
{
    public class UsersCreateValidator : AbstractValidator<UsersCreateDto>
    {
        public UsersCreateValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0)
                .WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

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

            RuleFor(x => x.UsersCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("El codigo de usuario es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(50)
                    .WithMessage("El codigo de usuario no debe exceder los 50 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches("^[a-zA-Z0-9_\\-]+$")
                    .WithMessage("El codigo de usuario solo puede contener letras, números, guiones y guiones bajos.")
                    .WithErrorCode(ErrorCodes.ValidationUsernameUnsafe)

                .Must(InputValidationHelpers.IsSafe)
                 .WithMessage("El codigo de usuario contiene caracteres peligrosos.")
                 .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.DocumentTypeId)
                .GreaterThan(0)
                .WithMessage("El id del documento es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

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

            RuleFor(x => x.ProfilesId)
               .GreaterThan(0)
               .WithMessage("El id del perfil es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

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
