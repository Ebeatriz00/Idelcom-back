using Application.Common.Security;
using Application.DTOs.Auth;
using FluentValidation;
using SharedKernel.Constants;
using System.ComponentModel.DataAnnotations;

namespace Application.Validators.Auth
{
    public class AuthValidator : AbstractValidator<AuthRequestDto>
    {
        private const int MaxUserKeyLen = 64;
        private static string T(string? s) => (s ?? string.Empty).Trim(); // trim seguro

        public AuthValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            // USERS KEY (reglas generales)
            RuleFor(x => x.UsersKey)
                .Must(s => !string.IsNullOrWhiteSpace(s))
                    .WithMessage("El usuario es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty)
                .Must(s => T(s).Length <= MaxUserKeyLen)
                    .WithMessage($"El usuario no debe exceder {MaxUserKeyLen} caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)
                .Must(s => !InputGuard.HasControlOrZeroWidth(T(s)))
                    .WithMessage("El usuario contiene caracteres no permitidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
                .Must(s => InputGuard.IsSafeUserKey(T(s)))
                    .WithMessage("El usuario contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
                .Must(s => !InputGuard.ContainsDangerousSqlTokens(T(s)))
                    .WithMessage("El usuario contiene secuencias peligrosas.")
                    .WithErrorCode(ErrorCodes.ValidationIllegalChar); ;

            // Si trae '@', valida formato de email (sin obligarlo para otros casos)
            When(x => T(x.UsersKey).Contains('@'), () =>
            {
                RuleFor(x => x.UsersKey)
                    .Must(s => new EmailAddressAttribute().IsValid(T(s)))
                    .WithMessage("El correo electrónico no es válido.")
                    .WithErrorCode(ErrorCodes.ValidationEmailInvalid);

            });

            // Si es solo dígitos, exige DNI (8) o RUC (11)
            When(x => InputGuard.IsDigits(T(x.UsersKey)), () =>
            {
                RuleFor(x => x.UsersKey)
                    .Must(s => {
                        var t = T(s);
                        return t.Length == 8 || t.Length == 11;
                    })
                    .WithMessage("Si ingresas documento, debe ser DNI (8) o RUC (11) dígitos.")
                    .WithErrorCode(ErrorCodes.ValidationLength);
                    
            });

            // PASSWORD
            RuleFor(x => x.UsersPassword)
                .Must(s => !string.IsNullOrEmpty(s))
                    .WithMessage("La contraseña es obligatoria.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty)
                .Must(s => T(s).Length >= 8)
                    .WithMessage("La contraseña debe tener al menos 8 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)
                .Must(s => T(s).Length <= 128)
                    .WithMessage("La contraseña no debe exceder 128 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)
                .Must(s => !InputGuard.HasControlOrZeroWidth(T(s)))
                    .WithMessage("La contraseña contiene caracteres no permitidos.")
                    .WithErrorCode(ErrorCodes.ValidationIllegalChar); ;
        }
    }
}
