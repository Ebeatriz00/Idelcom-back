using Application.DTOs.CostCenters;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.CostCenters
{
    public class CostCentersStatusToggleValidator : AbstractValidator<CostCentersStatusToggleDto>
    {
        public CostCentersStatusToggleValidator()
        {
            RuleFor(x => x.CostCentersId)
                .GreaterThan(0).WithMessage("ID inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.Status)
                .Must(status => status == "1" || status == "0")
                .WithMessage("Estado inválido. Solo se permite '1' (Activo) o '0' (Inactivo).")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0)
                .WithMessage("El usuario es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
