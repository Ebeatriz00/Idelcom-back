using Application.DTOs.ConceptGroups;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ConceptGroups
{
    public class ConceptGroupsCreateValidator : AbstractValidator<ConceptGroupsCreateDto>
    {
        public ConceptGroupsCreateValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ConceptTypeId)
                .GreaterThan(0).WithMessage("El tipo de concepto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(10).WithMessage("El código no debe exceder los 10 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches("^[a-zA-Z0-9]+$")
                .WithMessage("El código solo puede contener letras y números.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(60).WithMessage("La descripción no debe exceder los 60 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);
        }
    }
}
