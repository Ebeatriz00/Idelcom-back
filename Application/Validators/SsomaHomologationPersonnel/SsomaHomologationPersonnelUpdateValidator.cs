using Application.DTOs.SsomaHomologationPersonnel;
using FluentValidation;

namespace Application.Validators.SsomaHomologationPersonnel
{
    public class SsomaHomologationPersonnelUpdateValidator : AbstractValidator<SsomaHomologationPersonnelUpdateDto>
    {
        public SsomaHomologationPersonnelUpdateValidator()
        {
            RuleFor(x => x.HomologationPersonnelId)
                .GreaterThan(0)
                .WithMessage("El registro de homologacion de personal es obligatorio.");

            RuleFor(x => x.HomologationScopeId)
                .Must(x => x == 1 || x == 2)
                .WithMessage("El alcance de homologacion debe ser general o por operacion.");

            RuleFor(x => x.WorkerId)
                .GreaterThan(0)
                .WithMessage("El trabajador es obligatorio.");

            RuleFor(x => x.WorkerStatusId)
                .GreaterThan(0)
                .WithMessage("El estado del trabajador es obligatorio.");

            RuleFor(x => x.OperationsId)
                .GreaterThan(0)
                .When(x => x.OperationsId.HasValue)
                .WithMessage("La operacion indicada no es valida.");

            RuleFor(x => x.OperationsId)
                .NotNull()
                .When(x => x.HomologationScopeId == 2)
                .WithMessage("La operacion es obligatoria para homologacion por operacion.");

            RuleFor(x => x.OperationsId)
                .Null()
                .When(x => x.HomologationScopeId == 1)
                .WithMessage("La operacion no aplica para homologacion general.");

            RuleFor(x => x.MedicalAptitudeId)
                .GreaterThan(0)
                .When(x => x.HomologationScopeId == 2)
                .WithMessage("La aptitud medica es obligatoria.");

            RuleFor(x => x.ValidFrom)
                .NotEmpty()
                .WithMessage("La fecha de inicio de vigencia es obligatoria.");

            RuleFor(x => x.ValidTo)
                .NotEmpty()
                .WithMessage("La fecha de fin de vigencia es obligatoria.");

            RuleFor(x => x)
                .Must(x => x.ValidTo >= x.ValidFrom)
                .WithMessage("La fecha fin de vigencia no puede ser menor que la fecha inicial.");

            RuleFor(x => x.Notes)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Las notas son obligatorias.")
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres.");
        }
    }
}
