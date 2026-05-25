using Application.DTOs.SsomaOperationsRequirement;
using FluentValidation;

namespace Application.Validators.SsomaOperationsRequirement
{
    public class SsomaOperationsRequirementUpdateValidator : AbstractValidator<SsomaOperationsRequirementUpdateDto>
    {
        public SsomaOperationsRequirementUpdateValidator()
        {
            RuleFor(x => x.SsomaOperationsRequirementId)
                .GreaterThan(0)
                .WithMessage("El registro SSOMA por operación es obligatorio.");

            RuleFor(x => x.OperationsId)
                .GreaterThan(0)
                .WithMessage("La operación es obligatoria.");

            RuleFor(x => x.RequirementId)
                .GreaterThan(0)
                .WithMessage("El requerimiento es obligatorio.");

            RuleFor(x => x.ValidDays)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ValidDays.HasValue)
                .WithMessage("Los días válidos no pueden ser negativos.");
        }
    }
}
