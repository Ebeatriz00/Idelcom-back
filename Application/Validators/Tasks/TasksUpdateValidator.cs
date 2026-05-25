using Application.DTOs.Tasks;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Tasks
{
    public class TasksUpdateValidator : AbstractValidator<TasksUpdateDto>
    {
        public TasksUpdateValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.StateTaskId)
                .GreaterThan(0).WithMessage("El estado de la tarea es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);


            RuleFor(x => x.WorkerId)
                .GreaterThanOrEqualTo(0).WithMessage("El ID del responsable no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(100).WithMessage("La descripción no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);
        }
    }
}
