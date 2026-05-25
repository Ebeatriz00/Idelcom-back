using Application.DTOs.Exercises;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Exercises
{
    public class ExercisesBlockToggleValidator : AbstractValidator<ExercisesBlockToggleDto>
    {
        public ExercisesBlockToggleValidator()
        {
            RuleFor(x => x.ExercisesId)
                .GreaterThan(0).WithMessage("ID de ejercicio inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0)
                .WithMessage("El usuario es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
