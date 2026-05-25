using Application.DTOs.SsomaHomologationPersonnel;
using Application.DTOs.SsomaHomologationPersonnelDocument;
using FluentValidation;

namespace Application.Validators.SsomaHomologationPersonnel
{
    public class SsomaHomologationPersonnelCreateOrchestratedValidator : AbstractValidator<SsomaHomologationPersonnelCreateOrchestratedDto>
    {
        public SsomaHomologationPersonnelCreateOrchestratedValidator()
        {
            RuleFor(x => x.HomologationPersonnel)
                .NotNull()
                .WithMessage("La homologación de personal es obligatoria.")
                .SetValidator(new SsomaHomologationPersonnelCreateValidator());

            RuleForEach(x => x.Documents)
                .SetValidator(new SsomaHomologationPersonnelDocumentCreateOrchestratedItemValidator());
        }

        private sealed class SsomaHomologationPersonnelDocumentCreateOrchestratedItemValidator
            : AbstractValidator<SsomaHomologationPersonnelDocumentCreateDto>
        {
            public SsomaHomologationPersonnelDocumentCreateOrchestratedItemValidator()
            {
                RuleFor(x => x.RequirementId)
                    .GreaterThan(0)
                    .WithMessage("El requerimiento SSOMA es obligatorio.");

                RuleFor(x => x.FileName)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("El nombre de archivo es obligatorio.")
                    .MaximumLength(255)
                    .WithMessage("El nombre de archivo no puede exceder 255 caracteres.");

                RuleFor(x => x.FileUrl)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("La URL del archivo es obligatoria.")
                    .MaximumLength(1000)
                    .WithMessage("La URL del archivo no puede exceder 1000 caracteres.");

                RuleFor(x => x.FilePath)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("La ruta del archivo es obligatoria.")
                    .MaximumLength(1000)
                    .WithMessage("La ruta del archivo no puede exceder 1000 caracteres.");

                RuleFor(x => x.ReviewDate)
                    .GreaterThan(DateTime.MinValue)
                    .When(x => x.ReviewDate.HasValue)
                    .WithMessage("La fecha de revisión no es válida.");

                RuleFor(x => x.Observation)
                    .MaximumLength(1000)
                    .WithMessage("La observación no puede exceder 1000 caracteres.");
            }
        }
    }
}
