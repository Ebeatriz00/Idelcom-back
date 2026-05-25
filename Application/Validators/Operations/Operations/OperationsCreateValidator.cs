using Application.DTOs.Operations.Operations;
using FluentValidation;

namespace Application.Validators.Operations.Operations
{
    public class OperationsCreateValidator : AbstractValidator<OperationsCreateDto>
    {
        public OperationsCreateValidator()
        {
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
