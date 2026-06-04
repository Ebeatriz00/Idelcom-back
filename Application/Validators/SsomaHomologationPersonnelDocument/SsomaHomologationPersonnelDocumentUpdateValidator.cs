using Application.DTOs.SsomaHomologationPersonnelDocument;
using FluentValidation;

namespace Application.Validators.SsomaHomologationPersonnelDocument
{
    public class SsomaHomologationPersonnelDocumentUpdateValidator : AbstractValidator<SsomaHomologationPersonnelDocumentUpdateDto>
    {
        public SsomaHomologationPersonnelDocumentUpdateValidator()
        {
            RuleFor(x => x.SsomaHomologationPersonnelDocumentId)
                .GreaterThan(0)
                .WithMessage("El documento de homologación de personal es obligatorio.");

            RuleFor(x => x.HomologationPersonnelId)
                .GreaterThan(0)
                .WithMessage("La homologación de personal es obligatoria.");

            RuleFor(x => x.RequirementId)
                .GreaterThan(0)
                .WithMessage("El requerimiento SSOMA es obligatorio.");

            RuleFor(x => x.ValidationStatusId)
                .GreaterThan(0)
                .WithMessage("El estado de validación es obligatorio.");

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
