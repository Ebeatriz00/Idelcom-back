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
    public class ExchangeRateCreateValidator : AbstractValidator<ExchangeRateCreateDto>
    {
        public ExchangeRateCreateValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.PurchaseType)
                .GreaterThan(0).WithMessage("El tipo de cambio de compra debe ser mayor a cero.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.SaleType)
                .GreaterThan(0).WithMessage("El tipo de cambio de venta debe ser mayor a cero.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.DateFxrate)
                .NotEmpty().WithMessage("La fecha es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("La fecha no puede ser futura.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
