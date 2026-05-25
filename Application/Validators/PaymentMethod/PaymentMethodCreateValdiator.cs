using Application.DTOs.PaymentMethod;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.PaymentMethod
{
    public class PaymentMethodCreateValidator : AbstractValidator<PaymentMethodCreateDto>
    {
        public PaymentMethodCreateValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.PMConditionId)
                .GreaterThan(0).WithMessage("La condición de pago es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.PMVisId)
                .GreaterThan(0).WithMessage("La visibilidad es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(70).WithMessage("La descripción no debe exceder los 70 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Days)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Los días son obligatorios.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(2).WithMessage("Los días no deben exceder los 2 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[0-9]+$")
                .WithMessage("Los días deben ser solo números.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);
        }
    }
}
