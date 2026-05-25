using Application.DTOs.LogisticsRequest;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.LogisticsRequest
{
    public class CreateLogisticsRequestFromSelectedSuggestionsValidator : AbstractValidator<CreateLogisticsRequestFromSelectedSuggestionsDto>
    {
        public CreateLogisticsRequestFromSelectedSuggestionsValidator()
        {
            RuleFor(x => x.QuotationId).GreaterThan(0).When(x => x.QuotationId.HasValue).WithMessage("La cotizacion no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.QuotationVerId).GreaterThan(0).When(x => x.QuotationVerId.HasValue).WithMessage("La version de cotizacion no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.WorkOrderId).GreaterThan(0).When(x => x.WorkOrderId.HasValue).WithMessage("La orden de trabajo no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleForEach(x => x.SuggestionIds).GreaterThan(0).WithMessage("La sugerencia no es valida.").WithErrorCode(ErrorCodes.ValidationCharacterNegative);
            RuleFor(x => x.Observation).MaximumLength(500).WithMessage("La observacion no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
            RuleFor(x => x.OfficeObservation).MaximumLength(500).WithMessage("La observacion de oficina no debe exceder los 500 caracteres.").WithErrorCode(ErrorCodes.ValidationLength);
        }
    }
}
