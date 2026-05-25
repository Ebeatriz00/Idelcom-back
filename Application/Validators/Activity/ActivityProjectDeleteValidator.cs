using Application.DTOs.Activity;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Activity
{
    public class ActivityProjectDeleteValidator: AbstractValidator<ActivityDeleteProjectDto>
    {
        public ActivityProjectDeleteValidator()
        {

            RuleFor(x => x.ProjectToken)
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
