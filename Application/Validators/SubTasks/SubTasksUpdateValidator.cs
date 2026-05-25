using Application.DTOs.SubTasks.Application.DTOs.SubTasks;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.SubTasks
{
    public class SubTasksUpdateValidator : AbstractValidator<SubTasksUpdateDto>
    {
        public SubTasksUpdateValidator()
        {
            RuleFor(x => x.LinkToken)
                .NotEmpty().WithMessage("El token de la sub-tarea es obligatorio.");

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El ID del negocio es obligatorio.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título no puede estar vacío.")
                .MaximumLength(100).WithMessage("El título no puede exceder los 100 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(400).WithMessage("La descripción no puede exceder los 400 caracteres.");

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.");
        }
    }
}
