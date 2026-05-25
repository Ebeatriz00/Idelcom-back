using Application.DTOs.Brands;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Brands
{
    public class BrandsStatusToggleValidator : AbstractValidator<BrandsStatusToggleDto>
    {
        public BrandsStatusToggleValidator()
        {
            RuleFor(x => x.BrandsId)
                .GreaterThan(0).WithMessage("ID de marca inválido.")
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
