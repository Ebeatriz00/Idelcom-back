using Application.DTOs.JobTitle;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.JobTitle
{

    public class JobTitleUpdateValidator : AbstractValidator<JobTitleUpdateDto>
    {
        public JobTitleUpdateValidator()
        {
            RuleFor(x => x.JobTitleId)
                .GreaterThan(0).WithMessage("ID de cargo inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.AreaId)
                .GreaterThan(0).WithMessage("El área es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción del cargo es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(300).WithMessage("La descripción no debe exceder los 300 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s,.\-áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción contiene caracteres especiales no permitidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("La descripción contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);
        }
    }
}
