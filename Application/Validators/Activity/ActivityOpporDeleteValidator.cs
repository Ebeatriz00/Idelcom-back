using Application.DTOs.Activity;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Application.Validators.Activity
{
    public class ActivityOpporDeleteValidator : AbstractValidator<ActivityOpporDeleteDto>
    {
        public ActivityOpporDeleteValidator()
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
