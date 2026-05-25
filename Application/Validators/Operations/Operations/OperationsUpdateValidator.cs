using Application.DTOs.Operations.Operations;
using FluentValidation;

namespace Application.Validators.Operations.Operations
{
    public class OperationsUpdateValidator : AbstractValidator<OperationsUpdateDto>
    {
        public OperationsUpdateValidator()
        {
            RuleFor(x => x.OperationsId)
                .GreaterThan(0)
                .WithMessage("El OperationsId no puede ser cero.");

            RuleFor(x => x.OpporId)
                .GreaterThan(0)
                .WithMessage("El OpporId no puede ser 0.");

            RuleFor(x => x.OperationsStatusId)
                .GreaterThan(0);

            RuleFor(x => x.RequeredSsoma)
                .NotNull()
                .WithMessage("RequiredSsoma es obligatorio.");
        }
    }
}
