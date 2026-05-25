using Application.DTOs.ExchangeRate;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ExchangeRate
{
    public class ExchangeRateStatusToggleValidator : AbstractValidator<ExchangeRateStatusToggleDto>
    {
        public ExchangeRateStatusToggleValidator()
        {
            RuleFor(x => x.ExchangeRateId)
                .GreaterThan(0).WithMessage("El ID del tipo de cambio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Status)
                .Cascade(CascadeMode.Stop)
                .Must(status => status == "1" || status == "0")
                .WithMessage("Estado inválido. Solo se permite '1' (Activo) o '0' (Inactivo)")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
