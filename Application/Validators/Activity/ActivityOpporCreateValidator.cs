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
    public class ActivityOpporCreateValidator : AbstractValidator<ActivityOpporCreateDto>
    {
        public ActivityOpporCreateValidator() {

            RuleFor(x => x.BusinessId)
               .GreaterThan(0).WithMessage("El negocio es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.OpporToken)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("La oportunidad es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.ActivityMessage)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("El mensaje es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationEmpty);


        }
    }
}
