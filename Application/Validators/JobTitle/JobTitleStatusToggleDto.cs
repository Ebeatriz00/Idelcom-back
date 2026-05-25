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


    public class JobTitleStatusToggleValidator : AbstractValidator<JobTitleStatusToggleDto>
    {
        public JobTitleStatusToggleValidator()
        {
            RuleFor(x => x.JobTitleId)
                .GreaterThan(0).WithMessage("ID de cargo inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.Status)
                .Cascade(CascadeMode.Stop)
                .Must(status => status == "1" || status == "0")
                .WithMessage("Estado inválido. Solo se permite '1' (Activo) o '0' (Inactivo).")
                .WithErrorCode(ErrorCodes.Conflict)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("El estado tiene caracteres especiales.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El estado contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.Usersby)
                .GreaterThan(0)
                .WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
