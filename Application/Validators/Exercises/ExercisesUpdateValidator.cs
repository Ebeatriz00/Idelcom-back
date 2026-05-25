using Application.DTOs.CostCenters;
using Application.DTOs.Exercises;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Exercises
{
    public class ExercisesUpdateValidator : AbstractValidator<ExercisesUpdateDto>
    {
        public ExercisesUpdateValidator()
        {
            RuleFor(x => x.ExercisesId)
                .GreaterThan(0).WithMessage("ID inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("La descripción del centro de costo es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50)
                .WithMessage("La descripción no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción contiene caracteres no permitidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("La descripción contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("La fecha es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
