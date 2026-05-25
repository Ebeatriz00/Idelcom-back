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
    public class TasksCreateValidator : AbstractValidator<TasksCreateDto>
    {
        public TasksCreateValidator()
        {
            RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("El negocio es obligatorio.")
            .WithErrorCode(ErrorCodes.ValidationCharacterNegative);


            RuleFor(x => x.StateTaskId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El estado de la tarea es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.WorkerId)
                .GreaterThanOrEqualTo(0).WithMessage("El responsable es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El título es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(100).WithMessage("El título no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(400).WithMessage("La descripción no debe exceder los 400 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);


        }
    }
}
