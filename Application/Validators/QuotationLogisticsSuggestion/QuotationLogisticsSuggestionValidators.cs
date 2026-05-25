using Application.DTOs.QuotationLogisticsSuggestion;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.QuotationLogisticsSuggestion
{
    public class GenerateQuotationLogisticsSuggestionValidator : AbstractValidator<GenerateQuotationLogisticsSuggestionDto>
    {
        public GenerateQuotationLogisticsSuggestionValidator()
        {
            RuleFor(x => x.QuotationId).GreaterThan(0).WithMessage("La cotizacion es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.QuotationVerId).GreaterThan(0).When(x => x.QuotationVerId.HasValue).WithMessage("La version de cotizacion no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }

    public class ListQuotationLogisticsSuggestionValidator : AbstractValidator<ListQuotationLogisticsSuggestionFilterDto>
    {
        public ListQuotationLogisticsSuggestionValidator()
        {
            RuleFor(x => x.QuotationId).GreaterThan(0).WithMessage("La cotizacion es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.QuotationVerId).GreaterThan(0).When(x => x.QuotationVerId.HasValue).WithMessage("La version de cotizacion no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ResourceTypeId).GreaterThan(0).When(x => x.ResourceTypeId.HasValue).WithMessage("El tipo de recurso no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.WorkOrderId).GreaterThan(0).When(x => x.WorkOrderId.HasValue).WithMessage("La orden de trabajo no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.Search).MaximumLength(100).WithMessage("La busqueda no debe exceder los 100 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class UpdateQuotationLogisticsSuggestionValidator : AbstractValidator<UpdateQuotationLogisticsSuggestionDto>
    {
        public UpdateQuotationLogisticsSuggestionValidator()
        {
            RuleFor(x => x.QuotationLogisticsSuggestionId).GreaterThan(0).WithMessage("La sugerencia es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ApprovedQuantity).GreaterThanOrEqualTo(0).WithMessage("La cantidad aprobada no puede ser negativa.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ApprovedQuantity).GreaterThan(0).When(x => x.IsSelected).WithMessage("La cantidad aprobada debe ser mayor a cero si la sugerencia esta seleccionada.").WithErrorCode(ErrorCodes.ValidationRange);
            RuleFor(x => x.ApprovedQuantity).LessThanOrEqualTo(x => x.PendingToRequestQuantity)
                .When(x => x.IsSelected && x.PendingToRequestQuantity > 0)
                .WithMessage("La cantidad aprobada no puede superar la cantidad pendiente por solicitar.")
                .WithErrorCode(ErrorCodes.ValidationRange);
            RuleFor(x => x.WorkOrderId).GreaterThan(0).When(x => x.WorkOrderId.HasValue).WithMessage("La orden de trabajo no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.OfficeObservation).MaximumLength(500).WithMessage("La observacion no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class AddManualQuotationLogisticsSuggestionValidator : AbstractValidator<AddManualQuotationLogisticsSuggestionDto>
    {
        public AddManualQuotationLogisticsSuggestionValidator()
        {
            RuleFor(x => x.QuotationId).GreaterThan(0).WithMessage("La cotizacion es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.QuotationVerId).GreaterThan(0).WithMessage("La version de cotizacion es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.WorkOrderId).GreaterThan(0).When(x => x.WorkOrderId.HasValue).WithMessage("La orden de trabajo no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.LogisticsResourceTypeId).GreaterThan(0).WithMessage("El tipo de recurso es obligatorio.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ProductsId).GreaterThan(0).When(x => x.ProductsId.HasValue).WithMessage("El producto no es valido.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.Description).NotEmpty().When(x => !x.ProductsId.HasValue).WithMessage("La descripcion es obligatoria si no se informa producto.").WithErrorCode(ErrorCodes.ValidationEmpty);
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("La descripcion no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.SuggestedQuantity).GreaterThan(0).WithMessage("La cantidad sugerida debe ser mayor a cero.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.ApprovedQuantity).GreaterThanOrEqualTo(0).WithMessage("La cantidad aprobada no puede ser negativa.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.OfficeObservation).MaximumLength(500).WithMessage("La observacion no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }

    public class AssignWorkOrderQuotationLogisticsSuggestionValidator : AbstractValidator<AssignWorkOrderQuotationLogisticsSuggestionDto>
    {
        public AssignWorkOrderQuotationLogisticsSuggestionValidator()
        {
            RuleFor(x => x.SuggestionId).GreaterThan(0).WithMessage("La sugerencia es obligatoria.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.WorkOrderId).GreaterThan(0).When(x => x.WorkOrderId.HasValue).WithMessage("La orden de trabajo no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
