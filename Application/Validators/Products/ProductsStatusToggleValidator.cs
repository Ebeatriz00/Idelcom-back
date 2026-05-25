using Application.DTOs.Products;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Products
{
    public class ProductsStatusToggleValidator : AbstractValidator<ProductsStatusToggleDto>
    {
        public ProductsStatusToggleValidator()
        {
            RuleFor(x => x.ProductsId)
                .NotEmpty().WithMessage("El id es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

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
