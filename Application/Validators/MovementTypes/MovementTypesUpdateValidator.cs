using Application.DTOs.MovementTypes;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.MovementTypes
{
    public class MovementTypesUpdateValidator : AbstractValidator<MovementTypesUpdateDto>
    {
        public MovementTypesUpdateValidator()
        {
            RuleFor(x => x.MovementTypesId)
                .GreaterThan(0).WithMessage("ID de tipo de movimiento inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.MovSunatId)
                .GreaterThan(0).WithMessage("El tipo SUNAT es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.MovPerId)
                .GreaterThan(0).WithMessage("La personalización es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.MovClasId)
                .GreaterThan(0).WithMessage("La clasificación es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .Length(3).WithMessage("El código debe tener 3 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9]+$")
                .WithMessage("El código solo debe contener letras y números.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50).WithMessage("La descripción no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);
        }
    }
}
