using Application.DTOs.Operations.OperationsWorkOrderActivity;
using FluentValidation;

namespace Application.Validators.Operations.OperationsWorkOrderActivity
{
    public class OperationsWorkOrderActivityUpdateValidator : AbstractValidator<OperationsWorkOrderActivityUpdateDto>
    {
        public OperationsWorkOrderActivityUpdateValidator()
        {
            RuleFor(x => x.ActivityId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.WorkOrderId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.ActivityName)
                .NotEmpty()
                .MaximumLength(255)
                .WithErrorCode("required");

            RuleFor(x => x.MeasurementUnitId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.ComplexityId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.TargetQuantity)
                .GreaterThan(0)
                .WithErrorCode("required");
        }
    }
}
