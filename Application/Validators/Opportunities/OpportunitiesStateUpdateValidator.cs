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
    public class OpportunitiesStateUpdateValidator : AbstractValidator<OpportunitiesStateUpdateDto>
    {
        public OpportunitiesStateUpdateValidator()
        {
            RuleFor(x => x.LinkToken)
                .NotEmpty().WithMessage("El id es obligatorio.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.StateOpporId)
               .GreaterThan(0).WithMessage("El estado es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.StateOpporId)
                .NotEmpty()
                .WithMessage("Debe seleccionar un estado.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.ReasonRejection)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty().WithMessage("La razón es obligatoria.")
                        .MaximumLength(350).WithMessage("La razón no debe exceder los 350 caracteres.")
                        .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                        .WithMessage("La razón contiene caracteres inválidos.")
                        .When(x => x.StateOpporId == 5);
                });

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
