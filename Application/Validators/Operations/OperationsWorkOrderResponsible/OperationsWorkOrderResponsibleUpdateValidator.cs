using Application.DTOs.Operations.OperationsWorkOrderResponsible;
using FluentValidation;

namespace Application.Validators.Operations.OperationsWorkOrderResponsible
{
    public class OperationsWorkOrderResponsibleUpdateValidator : AbstractValidator<OperationsWorkOrderResponsibleUpdateDto>
    {
        public OperationsWorkOrderResponsibleUpdateValidator()
        {
            RuleFor(x => x.WorkOrderResponsibleId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.WorkOrderId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.WorkerId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.IsMain)
                .NotNull();

        }
    }

}
