using Application.DTOs.InventoryStock;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.InventoryStock
{
    public class InventoryStockCreateValidator : AbstractValidator<InventoryStockCreateDto>
    {
        public InventoryStockCreateValidator()
        {
            RuleFor(x => x.ProductsId)
                .GreaterThan(0).WithMessage("El producto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.WarehouseId)
                .GreaterThan(0).WithMessage("El almacén es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad de stock no puede ser negativa.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.AverageCost)
                .GreaterThanOrEqualTo(0).WithMessage("El costo promedio no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.LastCost)
                .GreaterThanOrEqualTo(0).WithMessage("El último costo no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.LastOutputDate)
                .GreaterThanOrEqualTo(x => x.LastEntryDate)
                .When(x => x.LastEntryDate.HasValue && x.LastOutputDate.HasValue)
                .WithMessage("La fecha de última salida no puede ser menor a la fecha de última entrada.")
                .WithErrorCode(ErrorCodes.ValidationRange);
        }
    }
}
