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
    public class ActivityOpporChangeStateValidator : AbstractValidator<ActivityStateOpporDto>
    {
        public ActivityOpporChangeStateValidator()
        {


            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Status)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El estado es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
