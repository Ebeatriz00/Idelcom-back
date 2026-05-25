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
    public class TasksOpporDeleteValidator : AbstractValidator<TaksOpporDeleteDto>
    {
        public TasksOpporDeleteValidator()
        {
            RuleFor(x => x.OpporToken)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("La oportunidad es obligatorio.")
                  .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.LinkToken)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("El id es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty);
        }
    }
}
