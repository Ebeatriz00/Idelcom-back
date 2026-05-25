using Application.DTOs.PurchaseReceipt;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.PurchaseReceipt
{
    public class PurchaseReceiptCreateValidator : AbstractValidator<PurchaseReceiptCreateDto>
    {
        public PurchaseReceiptCreateValidator()
        {
            RuleFor(x => x.BusinessId).GreaterThan(0).WithMessage("La empresa es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.SuppliersId).GreaterThan(0).WithMessage("El proveedor es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage("El almacen es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ReceiptTypeId).InclusiveBetween(1, 2).WithMessage("El tipo de recepcion no es valido.").WithErrorCode(ErrorCodes.ValidationRange);
            RuleFor(x => x.ReceiptDate).NotEmpty().WithMessage("La fecha de recepcion es obligatoria.").WithErrorCode(ErrorCodes.ValidationEmpty);
            RuleFor(x => x.SupplierGuideNumber).MaximumLength(30).WithMessage("La guia del proveedor no debe exceder los 30 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.Observation).MaximumLength(500).WithMessage("La observacion no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.Details).NotEmpty().WithMessage("Debe registrar al menos un detalle.").WithErrorCode(ErrorCodes.ValidationEmpty);
            RuleForEach(x => x.Details).SetValidator(new PurchaseReceiptDetailCreateValidator());

            RuleFor(x => x)
                .Must(x => x.ReceiptTypeId != 2 || x.PurchaseOrderId.HasValue)
                .WithMessage("La conformidad de servicio requiere orden de compra.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x)
                .Must(x => x.ReceiptTypeId != 2 || x.Details.All(d => d.PurchaseOrderDetailId.HasValue && d.PurchaseOrderDetailId.Value > 0))
                .WithMessage("La conformidad de servicio requiere detalle de orden de compra en todas las lineas.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x)
                .Must(x => !x.PurchaseOrderId.HasValue || x.Details.All(d => d.PurchaseOrderDetailId.HasValue && d.PurchaseOrderDetailId.Value > 0))
                .WithMessage("Si la recepcion tiene orden de compra, todos los detalles deben indicar detalle de orden de compra.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x)
                .Must(x => x.PurchaseOrderId.HasValue || x.Details.All(d => !d.PurchaseOrderDetailId.HasValue || d.PurchaseOrderDetailId.Value <= 0))
                .WithMessage("Si la recepcion no tiene orden de compra, ningun detalle debe indicar detalle de orden de compra.")
                .WithErrorCode(ErrorCodes.ValidationRange);

            RuleFor(x => x.Details)
                .Must(details => details.GroupBy(d => d.ProductsId).All(g => g.Count() == 1))
                .WithMessage("No se permite repetir productos en la misma recepcion.")
                .WithErrorCode(ErrorCodes.ValidationRange);
        }
    }

    public class PurchaseReceiptDetailCreateValidator : AbstractValidator<PurchaseReceiptDetailCreateDto>
    {
        public PurchaseReceiptDetailCreateValidator()
        {
            RuleFor(x => x.PurchaseOrderDetailId).GreaterThan(0).When(x => x.PurchaseOrderDetailId.HasValue).WithMessage("El detalle de orden de compra no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ProductsId).GreaterThan(0).WithMessage("El producto es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.UomId).GreaterThan(0).When(x => x.UomId.HasValue).WithMessage("La unidad de medida no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.OrderedQuantity).GreaterThanOrEqualTo(0).WithMessage("La cantidad ordenada no puede ser negativa.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ReceivedQuantity).GreaterThan(0).WithMessage("La cantidad recibida debe ser mayor a cero.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.Observation).MaximumLength(500).WithMessage("La observacion del detalle no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class PurchaseReceiptRegularizeValidator : AbstractValidator<PurchaseReceiptRegularizeDto>
    {
        public PurchaseReceiptRegularizeValidator()
        {
            RuleFor(x => x.PurchaseReceiptId).GreaterThan(0).WithMessage("La recepcion de compra es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PurchaseOrderId).GreaterThan(0).WithMessage("La orden de compra es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }

    public class PurchaseReceiptListFilterValidator : AbstractValidator<PurchaseReceiptListFilterDto>
    {
        public PurchaseReceiptListFilterValidator()
        {
            RuleFor(x => x.BusinessId).GreaterThan(0).WithMessage("La empresa es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.WarehouseId).GreaterThan(0).When(x => x.WarehouseId.HasValue).WithMessage("El almacen no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.SuppliersId).GreaterThan(0).When(x => x.SuppliersId.HasValue).WithMessage("El proveedor no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PurchaseOrderId).GreaterThan(0).When(x => x.PurchaseOrderId.HasValue).WithMessage("La orden de compra no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ReceiptStatusId).GreaterThan(0).When(x => x.ReceiptStatusId.HasValue).WithMessage("El estado no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ReceiptTypeId).InclusiveBetween(1, 2).When(x => x.ReceiptTypeId.HasValue).WithMessage("El tipo de recepcion no es valido.").WithErrorCode(ErrorCodes.ValidationRange);
            RuleFor(x => x.Page).GreaterThan(0).WithMessage("El numero de pagina debe ser mayor a cero.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200).WithMessage("El tamano de pagina debe estar entre 1 y 200.").WithErrorCode(ErrorCodes.ValidationRange);
            RuleFor(x => x.Search).MaximumLength(100).WithMessage("La busqueda no debe exceder los 100 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x).Must(x => !x.DateFrom.HasValue || !x.DateTo.HasValue || x.DateFrom.Value.Date <= x.DateTo.Value.Date)
                .WithMessage("La fecha desde no puede ser mayor que la fecha hasta.")
                .WithErrorCode(ErrorCodes.ValidationRange);
        }
    }
}
