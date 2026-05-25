using Application.DTOs.PMCondition;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.PMCondition
{
    public class PMConditionStatusToggleValidator : AbstractValidator<PMConditionStatusToggleDto>
    {
        public PMConditionStatusToggleValidator()
        {
            RuleFor(x => x.PMConditionId)
                .GreaterThan(0).WithMessage("ID de condición de pago inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BussinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Status)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El estado es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .Must(status => status == "1" || status == "0")
                .WithMessage("Estado inválido. Solo se permite '1' (Activo) o '0' (Inactivo).")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
