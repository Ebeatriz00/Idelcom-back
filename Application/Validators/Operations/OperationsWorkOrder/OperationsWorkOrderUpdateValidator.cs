using Application.DTOs.Operations.OperationsWorkOrder;
using FluentValidation;

namespace Application.Validators.Operations.OperationsWorkOrder
{
    public class OperationsWorkOrderUpdateValidator : AbstractValidator<OperationsWorkOrderUpdateDto>
    {
        public OperationsWorkOrderUpdateValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .GreaterThan(0)
                .NotNull()
                .WithMessage("El WorkOrderId no puede ser cero ni nulo.");

            RuleFor(x => x.OrderStatusId)
                .GreaterThan(0)
                .NotNull()
                .WithMessage("El OrderStatusId no puede ser cero ni nulo.");

            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage("StartDate es obligatorio.");

            RuleFor(x => x.NeedLogistics)
                .NotNull()
                .WithMessage("NeedLogistics es obligatorio.");

            RuleFor(x => x.NeedSsoma)
                .NotNull()
                .WithMessage("NeedSsoma es obligatorio.");

            RuleFor(x => x.NeedAttendance)
                .NotNull()
                .WithMessage("NeedAttendance es obligatorio.");
        }
    }
}
