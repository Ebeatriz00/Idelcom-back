using Application.DTOs.Suppliers;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Suppliers
{
    public class SuppliersStatusToggleValidator : AbstractValidator<SuppliersStatusToggleDto>
    {
        public SuppliersStatusToggleValidator()
        {
            RuleFor(x => x.SuppliersId)
                .GreaterThan(0).WithMessage("ID de proveedor inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.Status)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El estado es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .Must(status => status == "1" || status == "0")
                .WithMessage("Estado inválido. Solo se permite '1' (Activo) o '0' (Inactivo).")
                .WithErrorCode(ErrorCodes.Conflict);
        }
    }
}
