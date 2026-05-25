using Application.DTOs.PurchaseOrder;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.PurchaseOrder
{
    public class PurchaseOrderCreateValidator : AbstractValidator<PurchaseOrderCreateDto>
    {
        public PurchaseOrderCreateValidator()
        {
            RuleFor(x => x.SuppliersId).GreaterThan(0).WithMessage("El proveedor es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PurchaseOrderDate).NotEmpty().WithMessage("La fecha de la orden de compra es obligatoria.").WithErrorCode(ErrorCodes.ValidationEmpty);
            RuleFor(x => x.CurrencyId).GreaterThan(0).WithMessage("La moneda es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ExchangeRate).GreaterThan(0).When(x => x.ExchangeRate.HasValue).WithMessage("El tipo de cambio debe ser mayor a cero.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PmConditionId).GreaterThan(0).When(x => x.PmConditionId.HasValue).WithMessage("La condicion de pago no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.WarehouseId).GreaterThan(0).When(x => x.WarehouseId.HasValue).WithMessage("El almacen no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.SupplierQuotationReferenceNumber).MaximumLength(100).WithMessage("El nro de referencia de cotizacion del proveedor no debe exceder los 100 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.References).MaximumLength(500).WithMessage("Las referencias no deben exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.Observation).MaximumLength(500).WithMessage("La observacion no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.Details).NotEmpty().WithMessage("Debe registrar al menos un detalle.").WithErrorCode(ErrorCodes.ValidationEmpty);
            RuleForEach(x => x.Details).SetValidator(new PurchaseOrderDetailCreateValidator());
        }
    }

    public class PurchaseOrderDetailCreateValidator : AbstractValidator<PurchaseOrderDetailCreateDto>
    {
        public PurchaseOrderDetailCreateValidator()
        {
            RuleFor(x => x.ProductsId).GreaterThan(0).WithMessage("El producto es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.UomId).GreaterThan(0).When(x => x.UomId.HasValue).WithMessage("La unidad de medida no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100).WithMessage("El descuento debe estar entre 0 y 100.").WithErrorCode(ErrorCodes.ValidationRange);
            RuleFor(x => x.TaxesId).GreaterThan(0).When(x => x.TaxesId.HasValue).WithMessage("El impuesto no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PriceIncludesTax).NotNull().WithMessage("Debe indicar si el precio incluye impuesto.").WithErrorCode(ErrorCodes.ValidationEmpty);
            RuleFor(x => x.Observation).MaximumLength(500).WithMessage("La observacion del detalle no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class PurchaseOrderUpdateValidator : AbstractValidator<PurchaseOrderUpdateDto>
    {
        public PurchaseOrderUpdateValidator()
        {
            RuleFor(x => x.PurchaseOrderId).GreaterThan(0).WithMessage("La orden de compra es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.SuppliersId).GreaterThan(0).WithMessage("El proveedor es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PurchaseOrderDate).NotEmpty().WithMessage("La fecha de la orden de compra es obligatoria.").WithErrorCode(ErrorCodes.ValidationEmpty);
            RuleFor(x => x.CurrencyId).GreaterThan(0).WithMessage("La moneda es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ExchangeRate).GreaterThan(0).When(x => x.ExchangeRate.HasValue).WithMessage("El tipo de cambio debe ser mayor a cero.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PmConditionId).GreaterThan(0).When(x => x.PmConditionId.HasValue).WithMessage("La condicion de pago no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.WarehouseId).GreaterThan(0).When(x => x.WarehouseId.HasValue).WithMessage("El almacen no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.SupplierQuotationReferenceNumber).MaximumLength(100).WithMessage("El nro de referencia de cotizacion del proveedor no debe exceder los 100 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.References).MaximumLength(500).WithMessage("Las referencias no deben exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.Observation).MaximumLength(500).WithMessage("La observacion no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.Details).NotEmpty().WithMessage("Debe registrar al menos un detalle.").WithErrorCode(ErrorCodes.ValidationEmpty);
            RuleForEach(x => x.Details).SetValidator(new PurchaseOrderDetailUpdateValidator());
        }
    }

    public class PurchaseOrderDetailUpdateValidator : AbstractValidator<PurchaseOrderDetailUpdateDto>
    {
        public PurchaseOrderDetailUpdateValidator()
        {
            RuleFor(x => x.PurchaseOrderDetailId).GreaterThan(0).When(x => x.PurchaseOrderDetailId.HasValue).WithMessage("El detalle de la orden de compra no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ProductsId).GreaterThan(0).When(x => x.IsActive).WithMessage("El producto es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.UomId).GreaterThan(0).When(x => x.UomId.HasValue).WithMessage("La unidad de medida no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.Quantity).GreaterThan(0).When(x => x.IsActive).WithMessage("La cantidad debe ser mayor a cero.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100).WithMessage("El descuento debe estar entre 0 y 100.").WithErrorCode(ErrorCodes.ValidationRange);
            RuleFor(x => x.TaxesId).GreaterThan(0).When(x => x.TaxesId.HasValue).WithMessage("El impuesto no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PriceIncludesTax).NotNull().WithMessage("Debe indicar si el precio incluye impuesto.").WithErrorCode(ErrorCodes.ValidationEmpty);
            RuleFor(x => x.Observation).MaximumLength(500).WithMessage("La observacion del detalle no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class PurchaseOrderApproveValidator : AbstractValidator<PurchaseOrderApproveDto>
    {
        public PurchaseOrderApproveValidator()
        {
            RuleFor(x => x.PurchaseOrderId).GreaterThan(0).WithMessage("La orden de compra es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ApprovedBy).GreaterThan(0).WithMessage("El usuario aprobador es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }

    public class PurchaseOrderCancelValidator : AbstractValidator<PurchaseOrderCancelDto>
    {
        public PurchaseOrderCancelValidator()
        {
            RuleFor(x => x.PurchaseOrderId).GreaterThan(0).WithMessage("La orden de compra es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.CancelledBy).GreaterThan(0).WithMessage("El usuario que anula es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.Reason).MaximumLength(500).WithMessage("El motivo no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class PurchaseOrderSendForApprovalValidator : AbstractValidator<PurchaseOrderSendForApprovalDto>
    {
        public PurchaseOrderSendForApprovalValidator()
        {
            RuleFor(x => x.PurchaseOrderId).GreaterThan(0).WithMessage("La orden de compra es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }

    public class PurchaseOrderListFilterValidator : AbstractValidator<PurchaseOrderListFilterDto>
    {
        public PurchaseOrderListFilterValidator()
        {
            RuleFor(x => x.SuppliersId).GreaterThan(0).When(x => x.SuppliersId.HasValue).WithMessage("El proveedor no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PurchaseOrderStatusId).GreaterThan(0).When(x => x.PurchaseOrderStatusId.HasValue).WithMessage("El estado no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("El numero de pagina debe ser mayor a cero.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200).WithMessage("El tamano de pagina debe estar entre 1 y 200.").WithErrorCode(ErrorCodes.ValidationRange);
            RuleFor(x => x.Search).MaximumLength(100).WithMessage("La busqueda no debe exceder los 100 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x).Must(x => !x.DateFrom.HasValue || !x.DateTo.HasValue || x.DateFrom.Value.Date <= x.DateTo.Value.Date)
                .WithMessage("La fecha desde no puede ser mayor que la fecha hasta.")
                .WithErrorCode(ErrorCodes.ValidationRange);
        }
    }

    public class PurchaseOrderAttachInvoiceValidator : AbstractValidator<PurchaseOrderAttachInvoiceDto>
    {
        public PurchaseOrderAttachInvoiceValidator()
        {
            RuleFor(x => x.PurchaseOrderId).GreaterThan(0).WithMessage("La orden de compra es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.SupplierInvoiceId).GreaterThan(0).WithMessage("La factura proveedor es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.RegularizationReason).MaximumLength(500).WithMessage("El motivo de regularizacion no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class PurchaseOrderCreateFromInvoiceValidator : AbstractValidator<PurchaseOrderCreateFromInvoiceDto>
    {
        public PurchaseOrderCreateFromInvoiceValidator()
        {
            RuleFor(x => x.SupplierInvoiceId).GreaterThan(0).WithMessage("La factura proveedor es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.WarehouseId).GreaterThan(0).When(x => x.WarehouseId.HasValue).WithMessage("El almacen no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.Observation).MaximumLength(500).WithMessage("La observacion no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }
}
