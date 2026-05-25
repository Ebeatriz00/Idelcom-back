using Application.DTOs.FileTrackingProducts;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.FileTrackingProducts
{


    public class FileTrackingProductsDeleteValidator : AbstractValidator<FileTrackingProductsDeleteDto>
    {
        public FileTrackingProductsDeleteValidator()
        {
            RuleFor(x => x.FileTrackingProductsId)
                .GreaterThan(0).WithMessage("El identificador del archivo es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ProductsId)
                .NotEmpty().WithMessage("El token del producto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);
        }
    }
}
