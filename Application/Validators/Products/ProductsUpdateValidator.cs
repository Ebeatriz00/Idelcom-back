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
    public class ProductsUpdateValidator : AbstractValidator<ProductsUpdateDto>
    {
        public ProductsUpdateValidator()
        {
            RuleFor(x => x.ProductsId)
                .GreaterThan(0).WithMessage("El producto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);


            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(200).WithMessage("La descripción no debe exceder los 200 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.ShortDescription)
                .MaximumLength(100).WithMessage("La descripción corta no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.Barcode)
                .MaximumLength(100).WithMessage("El código de barras no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.PartNum)
                .MaximumLength(100).WithMessage("El número de parte no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.ProductTypeId)
                .GreaterThan(0).WithMessage("El tipo de producto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.CategoriesId)
                .GreaterThan(0).WithMessage("La categoría es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ProductLinesId)
                .GreaterThan(0).WithMessage("La línea de producto es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.BrandsId)
                .GreaterThan(0).WithMessage("La marca es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UomId)
                .GreaterThan(0).WithMessage("La unidad de medida es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.SunatId)
                .GreaterThanOrEqualTo(0).WithMessage("El código SUNAT no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.StockMin)
                .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.StockMax)
                .GreaterThanOrEqualTo(0).WithMessage("El stock máximo no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x)
                .Must(x => x.StockMax == 0 || x.StockMax >= x.StockMin)
                .WithMessage("El stock máximo debe ser mayor o igual al stock mínimo.")
                .WithErrorCode(ErrorCodes.ValidationInvalid);

            RuleFor(x => x.ConversionFactor)
                .GreaterThanOrEqualTo(0).WithMessage("El factor de conversión no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0).WithMessage("El peso no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Volume)
                .GreaterThanOrEqualTo(0).WithMessage("El volumen no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Files)
                .MaximumLength(350).WithMessage("La ruta del archivo no debe exceder los 350 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x)
                .Must(x => !(x.ManageLots && x.ManegesSerials))
                .WithMessage("El producto no puede manejar lotes y series al mismo tiempo.")
                .WithErrorCode(ErrorCodes.ValidationInvalid);

            RuleFor(x => x)
                .Must(x => !x.ExpirationControl || x.ManageLots)
                .WithMessage("Para controlar vencimiento, el producto debe manejar lotes.")
                .WithErrorCode(ErrorCodes.ValidationInvalid);

            RuleFor(x => x)
                .Must(x => !x.IsServices || !x.IsStockable)
                .WithMessage("Un servicio no debe manejar stock.")
                .WithErrorCode(ErrorCodes.ValidationInvalid);

            RuleFor(x => x)
                .Must(x => !x.IsTool || x.IsReturnable)
                .WithMessage("Una herramienta debe ser retornable.")
                .WithErrorCode(ErrorCodes.ValidationInvalid);

            RuleFor(x => x)
                .Must(x => !x.IsTool || !x.CanSell)
                .WithMessage("Una herramienta no debe marcarse como vendible.")
                .WithErrorCode(ErrorCodes.ValidationInvalid);

            RuleFor(x => x)
                .Must(x => x.CanBuy || x.CanSell || x.IsStockable)
                .WithMessage("El producto debe poder comprarse, venderse o manejar stock.")
                .WithErrorCode(ErrorCodes.ValidationInvalid);
        }
    }
}
