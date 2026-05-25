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
    public class OpportunitiesClientsUpdateValidator : AbstractValidator<OpportunitiesClientsUpdateDto>
    {
        public OpportunitiesClientsUpdateValidator()
        {
            RuleFor(x => x.LinkToken)
                .NotEmpty().WithMessage("El id es obligatorio.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ClientsId)
               .GreaterThan(0).WithMessage("El cliente es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ReasonRejection)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La razón es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(350).WithMessage("La razón no debe exceder los 350 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La razón contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
