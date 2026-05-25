using Application.DTOs.SubTasks;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.SubTasks
{
    public class SubTasksDeleteValidator : AbstractValidator<SubTasksDeleteDto>
    {
        public SubTasksDeleteValidator()
        {
            RuleFor(x => x.LinkToken)
                .NotEmpty().WithMessage("El token de la sub-tarea es obligatorio.");

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que elimina es obligatorio.");
        }
    }
}
