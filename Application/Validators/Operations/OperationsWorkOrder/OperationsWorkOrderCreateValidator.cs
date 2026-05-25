using Application.DTOs.Operations.OperationsWorkOrder;
using FluentValidation;

namespace Application.Validators.Operations.OperationsWorkOrder
{
    public class OperationsWorkOrderCreateValidator : AbstractValidator<OperationsWorkOrderCreateDto>
    {
        public OperationsWorkOrderCreateValidator()
        {
            RuleFor(x => x.OperationsId)
                .GreaterThan(0);
            RuleFor(x => x.OrderStatusId)
                .GreaterThan(0);
            RuleFor(x => x.StartDate)
                .NotNull();
            RuleFor(x => x.NeedLogistics)
                .NotNull();
        }
    }
}
