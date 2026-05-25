using Application.DTOs.Operations.OperationsWorkOrderProgress;
using FluentValidation;

namespace Application.Validators.Operations.OperationsWorkOrderProgress
{
    public class OperationsWorkOrderProgressCreateValidator : AbstractValidator<OperationsWorkOrderProgressCreateDto>
    {
        public OperationsWorkOrderProgressCreateValidator()
        {
            RuleFor(x => x.ActivityId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.ReportedQuantity)
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.ReportedDate)
                .NotEmpty()
                .WithErrorCode("required");

        }
    }
}
