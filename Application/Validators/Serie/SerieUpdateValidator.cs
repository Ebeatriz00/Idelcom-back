using Application.DTOs.Series;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Series
{


    public class SeriesUpdateValidator : AbstractValidator<SeriesUpdateDto>
    {
        public SeriesUpdateValidator()
        {
            RuleFor(x => x.SeriesId)
                .GreaterThan(0).WithMessage("ID de serie inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.PaymentTypeId)
                .GreaterThan(0).WithMessage("El tipo de pago es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.SeriesName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la serie es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(20).WithMessage("El nombre de la serie no debe exceder los 20 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s,.\-áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("El nombre de la serie contiene carateres especiales.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Correlative)
                .GreaterThanOrEqualTo(0).WithMessage("El correlativo debe ser un número válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Used)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El indicador de uso es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .Length(1).WithMessage("El indicador de uso debe ser un solo caracter.")
                .WithErrorCode(ErrorCodes.ValidationLength);
        }
    }
}
