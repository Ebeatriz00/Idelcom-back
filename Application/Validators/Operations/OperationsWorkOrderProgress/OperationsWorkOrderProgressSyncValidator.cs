using Application.DTOs.Operations.OperationsWorkOrderProgress;
using Application.Common.Validations;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.Operations.OperationsWorkOrderProgress
{
    public class OperationsWorkOrderProgressSyncValidator : AbstractValidator<OperationsWorkOrderProgressSyncDto>
    {
        public OperationsWorkOrderProgressSyncValidator()
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

            RuleFor(x => x.Photos)
                .Must(x => x == null || x.Count <= 5)
                .WithMessage("No se pueden subir más de 5 fotos por registro.")
                .WithErrorCode("max_limit");

            RuleForEach(x => x.Photos)
                .MustBeValidImage()
                .MaxSizeInBytes(5 * FileSize.MB);
        }
    }
}
