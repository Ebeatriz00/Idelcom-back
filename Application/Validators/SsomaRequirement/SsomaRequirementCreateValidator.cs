using Application.DTOs.SsomaRequirement;
using FluentValidation;

namespace Application.Validators.SsomaRequirement
{
    public class SsomaRequirementCreateValidator : AbstractValidator<SsomaRequirementCreateDto>
    {
        public SsomaRequirementCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre del requerimiento es obligatorio.")
                .MaximumLength(200)
                .WithMessage("El nombre del requerimiento no puede exceder 200 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("La descripción es obligatoria.")
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder 1000 caracteres.");

            RuleFor(x => x.ScopeId)
                .GreaterThan(0)
                .WithMessage("El alcance es obligatorio.");

            
        }
    }
}
