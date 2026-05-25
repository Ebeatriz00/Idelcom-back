using Application.DTOs.Area;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Area
{
    public class AreaUpdateValidator : AbstractValidator<AreaUpdateDto>
    {
        public AreaUpdateValidator()
        {
            RuleFor(x => x.AreaId)
                .GreaterThan(0).WithMessage("ID inválido")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción del área es obligatoria.")
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
