using Application.DTOs.InventoryCount;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.InventoryCount
{
    public class InventoryCountCreateValidator : AbstractValidator<InventoryCountCreateDto>
    {
        public InventoryCountCreateValidator()
        {
            RuleFor(x => x.WarehouseId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un almacén.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.CountDate)
                .NotNull()
                .WithMessage("La fecha de conteo es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.Observation)
                .MaximumLength(500)
                .WithMessage("La observación no debe exceder los 500 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class InventoryCountUpdateDetailsValidator : AbstractValidator<InventoryCountUpdateDetailsDto>
    {
        public InventoryCountUpdateDetailsValidator()
        {
            RuleFor(x => x.InventoryCountId)
                .GreaterThan(0)
                .WithMessage("La toma de inventario es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Details)
                .NotEmpty()
                .WithMessage("Debe ingresar al menos un detalle.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleForEach(x => x.Details).SetValidator(new InventoryCountDetailUpdateValidator());
        }
    }

    public class InventoryCountDetailUpdateValidator : AbstractValidator<InventoryCountDetailUpdateDto>
    {
        public InventoryCountDetailUpdateValidator()
        {
            RuleFor(x => x.InventoryCountDetailId)
                .GreaterThan(0)
                .WithMessage("El detalle de inventario no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.CountedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad contada no puede ser negativa.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.LotNumber)
                .MaximumLength(100)
                .WithMessage("El número de lote no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.SerialNumber)
                .MaximumLength(100)
                .WithMessage("El número de serie no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.Observation)
                .MaximumLength(500)
                .WithMessage("La observación del detalle no debe exceder los 500 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class InventoryCountListFilterValidator : AbstractValidator<InventoryCountListFilterDto>
    {
        public InventoryCountListFilterValidator()
        {
            RuleFor(x => x.WarehouseId)
                .GreaterThan(0)
                .When(x => x.WarehouseId.HasValue)
                .WithMessage("El almacén no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.InventoryCountStatusId)
                .GreaterThan(0)
                .When(x => x.InventoryCountStatusId.HasValue)
                .WithMessage("El estado no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("El número de página debe ser mayor a cero.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 200)
                .WithMessage("El tamaño de página debe estar entre 1 y 200.")
                .WithErrorCode(ErrorCodes.ValidationRange);

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage("La búsqueda no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x)
                .Must(x => !x.DateFrom.HasValue || !x.DateTo.HasValue || x.DateFrom.Value.Date <= x.DateTo.Value.Date)
                .WithMessage("La fecha desde no puede ser mayor que la fecha hasta.")
                .WithErrorCode(ErrorCodes.ValidationRange);
        }
    }
}
