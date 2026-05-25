using Application.DTOs.WarehousesMovement;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.WarehousesMovement
{
    public class WarehousesMovementCreateValidator : AbstractValidator<WarehousesMovementCreateDto>
    {
        public WarehousesMovementCreateValidator()
        {
            RuleFor(x => x.MovementTypeId)
                .GreaterThan(0).WithMessage("El tipo de movimiento es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.WarehouseId)
                .GreaterThan(0).WithMessage("El almacén es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.WarehouseDestinationId)
                .GreaterThanOrEqualTo(0).When(x => x.WarehouseDestinationId.HasValue)
                .WithMessage("El almacén destino no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.SuppliersId)
                .GreaterThanOrEqualTo(0).When(x => x.SuppliersId.HasValue)
                .WithMessage("El proveedor no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ClientsId)
                .GreaterThanOrEqualTo(0).When(x => x.ClientsId.HasValue)
                .WithMessage("El cliente no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Series)
                .MaximumLength(20).WithMessage("La serie no debe exceder los 20 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.NumberDocument)
                .MaximumLength(50).WithMessage("El número de documento no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.ReferenceDocument)
                .MaximumLength(50).WithMessage("El número de referencia no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.Observation)
                .MaximumLength(500).WithMessage("La observación no debe exceder los 500 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.TaxesId)
                .GreaterThan(0).When(x => x.TaxesId.HasValue)
                .WithMessage("El impuesto no es valido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Details)
                .NotEmpty().WithMessage("Debe registrar al menos un detalle del movimiento.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleForEach(x => x.Details).SetValidator(new WarehousesMovementDetailCreateValidator());
        }
    }

    public class WarehousesMovementDetailCreateValidator : AbstractValidator<WarehousesMovementDetailCreateDto>
    {
        public WarehousesMovementDetailCreateValidator()
        {
            RuleFor(x => x.ProductsId)
                .GreaterThan(0).WithMessage("El producto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UnitCost)
                .GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.LotNumber)
                .MaximumLength(50).WithMessage("El lote no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.SerialNumber)
                .MaximumLength(100).WithMessage("El número de serie no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.Observation)
                .MaximumLength(500).WithMessage("La observación del detalle no debe exceder los 500 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);
        }
    }
}
