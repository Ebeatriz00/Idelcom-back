using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.FileTrackingProducts
{
    using Application.DTOs.FileTrackingProducts;
    using FluentValidation;
    using SharedKernel.Constants;

    public class FileTrackingProductsCreateValidator : AbstractValidator<FileTrackingProductsCreateDto>
    {
        public FileTrackingProductsCreateValidator()
        {
            RuleFor(x => x.ProductsId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
                       
        }
    }
}
