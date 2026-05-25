using Application.DTOs.UsersPreferences;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.UsersPreferences
{
    public class UsersPreferencesUpdateValidator : AbstractValidator<UsersPrefeUpdateDto>
    {
        public UsersPreferencesUpdateValidator() {
            RuleFor(x => x.UsersId)
            .GreaterThan(0).WithMessage("ID inválido")
            .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("BusinessId inválido")
            .WithErrorCode(ErrorCodes.Conflict);
        }
    }
}
