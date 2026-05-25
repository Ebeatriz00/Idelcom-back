using Application.DTOs.Business;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Validators.Business
{
    public class BusinessCreateValidator : AbstractValidator<BusinessCreateDto>
    {
        // Patrones de formato
        private static readonly Regex LicenseRegex = new(@"^[A-Z0-9]{5}(?:-[A-Z0-9]{5}){3}$", RegexOptions.Compiled);


        public BusinessCreateValidator()
        {
            RuleFor(x => x.BusinessName)
                .NotEmpty()
                .WithMessage("El nombre de la empresa es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MinimumLength(3)
                .WithMessage("El nombre de la empresa debe tener al menos 3 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .MaximumLength(150)
                .WithMessage("El nombre de la empresa no debe exceder 150 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("La descripción contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);


            RuleFor(x => x.BusinessRuc)
                .NotEmpty().WithMessage("El RUC es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .Length(11).WithMessage("El RUC debe tener 11 dígitos.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^\d{11}$").WithMessage("El RUC debe contener solo dígitos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
                .Must(IsValidPeruRuc).WithMessage("El RUC no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.BusinessPhone)
                .Cascade(CascadeMode.Stop)
                .Must(p => string.IsNullOrWhiteSpace(p) || Regex.IsMatch(p, @"^\+?\d{7,15}$"))
                .WithMessage("El teléfono debe contener entre 7 y 15 dígitos (con o sin prefijo +).")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            // === Licencia del sistema ===
            RuleFor(x => x.License)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("La licencia es obligatoria.")
                 .WithErrorCode(ErrorCodes.ValidationEmpty)
                 .Must(k => LicenseRegex.IsMatch((k ?? string.Empty).Trim().ToUpper()))
                     .WithMessage("La licencia debe tener el formato #####-#####-#####-##### (alfa-num mayúsculas).")
                     .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

        }

        // === RUC Perú (módulo 11) ===
        private static bool IsValidPeruRuc(string? ruc)
        {
            if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != 11 || !ruc.All(char.IsDigit)) return false;

            // Prefijos comunes: 10 (PN), 15 (rég. especial), 17 (otros), 20 (PJ)
            var prefix = ruc.Substring(0, 2);
            if (prefix is not ("10" or "15" or "17" or "20")) return false;

            int[] weights = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int sum = 0;
            for (int i = 0; i < 10; i++) sum += (ruc[i] - '0') * weights[i];
            int remainder = sum % 11;
            int check = 11 - remainder;
            if (check == 10) check = 0;
            else if (check == 11) check = 1;

            return check == (ruc[10] - '0');
        }
    }

}
