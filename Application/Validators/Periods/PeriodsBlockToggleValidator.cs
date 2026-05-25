using Application.DTOs.Periods;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Periods
{
    public class PeriodsBlockToggleValidator : AbstractValidator<PeriodsBlockToggleDto>
    {
        public PeriodsBlockToggleValidator()
        {
            RuleFor(x => x.PeriodsId)
                .GreaterThan(0).WithMessage("ID de período inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
