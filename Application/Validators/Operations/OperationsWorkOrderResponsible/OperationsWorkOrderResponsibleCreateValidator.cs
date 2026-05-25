using Application.DTOs.Operations.OperationsWorkOrderResponsible;
using FluentValidation;

namespace Application.Validators.Operations.OperationsWorkOrderResponsible
{
    public class OperationsWorkOrderResponsibleCreateValidator : AbstractValidator<OperationsWorkOrderResponsibleCreateDto>
    {
        public OperationsWorkOrderResponsibleCreateValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.WorkerId)
                .NotEmpty()
                .GreaterThan(0)
                .WithErrorCode("required");

            RuleFor(x => x.IsMain)
                .NotNull().WithMessage("Es Requerido");
        }
    }

}
