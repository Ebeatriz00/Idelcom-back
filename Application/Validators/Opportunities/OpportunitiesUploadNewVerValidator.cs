using Application.DTOs.Opportunities;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Opportunities
{
    public class OpportunitiesUploadNewVerValidator : AbstractValidator<OpportunitiesUploadNewVerDto>
    {
        public OpportunitiesUploadNewVerValidator()
        {
            RuleFor(x => x.LinkToken)
                .NotEmpty().WithMessage("El id es obligatorio.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
           
            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
