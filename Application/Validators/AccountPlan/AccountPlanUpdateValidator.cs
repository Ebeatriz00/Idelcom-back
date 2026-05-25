using Application.DTOs.AccountPlan;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.AccountPlan
{
    public class AccountPlanUpdateValidator : AbstractValidator<AccountPlanUpdateDto>
    {
        public AccountPlanUpdateValidator()
        {
            RuleFor(x => x.AccountPlanId)
                    .GreaterThan(0).WithMessage("ID inválido")
                    .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.AccountCode)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("El código es obligatorio.")
                 .WithErrorCode(ErrorCodes.ValidationEmpty)

                 .MaximumLength(150)
                 .WithMessage("El código de no debe exceder de los 150 caracteres.")
                 .WithErrorCode(ErrorCodes.ValidationLength)

                 .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                 .WithMessage("El codigo tiene caracteres especiales.")
                 .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                 .Must(InputValidationHelpers.IsSafe)
                 .WithMessage("El código  contiene caracteres peligrosos.")
                 .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.AccountName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción  es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(350)
                .WithMessage("La descripción no debe exceder los 350 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                 .WithMessage("El codigo tiene caracteres especiales.")
                 .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("La descripción contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);
        }
        
    }
}
