using Application.DTOs.Viability;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Viability
{
    public class ViabilityUpdateValidator : AbstractValidator<ViabilityUpdateDto>
    {
        public ViabilityUpdateValidator()
        {
            RuleFor(x => x.linkToken)
                .NotEmpty().WithMessage("El id de la viabilidad es obligatorio.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.OpporToken)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La referencia de oportunidad es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.ViabilityDecision)
                .GreaterThan(0).WithMessage("La decisión de viabilidad es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Compliance)
                .GreaterThanOrEqualTo(0).WithMessage("El valor de cumplimiento es inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.NonCompliance)
                .GreaterThanOrEqualTo(0).WithMessage("El valor de incumplimiento es inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Authority)
                .NotEmpty().WithMessage("La autoridad de compra es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.Budget)
                .NotEmpty().WithMessage("El presupuesto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.Need)
                .NotEmpty().WithMessage("La necesidad de solución es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.Term)
                .NotEmpty().WithMessage("El término de contratación es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.WorkExperience)
                .NotEmpty().WithMessage("La experiencia laboral es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.StaffExperience)
                .NotEmpty().WithMessage("La experiencia del personal es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.CompanyExperience)
                .NotEmpty().WithMessage("La experiencia de la empresa es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.Ability)
                .NotEmpty().WithMessage("La habilidad técnica es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.Schedule)
                .NotEmpty().WithMessage("El cronograma es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
