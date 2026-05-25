using Application.DTOs.ProcessType;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ProcessType
{
    public class ProcessTypeUpdateValidator : AbstractValidator<ProcessTypeUpdateDto>
    {
        public ProcessTypeUpdateValidator() {
            RuleFor(x => x.ProcessTypeId)
                        .GreaterThan(0).WithMessage("ID inválido")
                        .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.DescType)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("La descripción es obligatorio.")
                 .WithErrorCode(ErrorCodes.ValidationEmpty)

                 .MaximumLength(150)
                 .WithMessage("La descripción no debe exceder de los 150 caracteres.")
                 .WithErrorCode(ErrorCodes.ValidationLength)

                 .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                 .WithMessage("La descripción tiene caracteres especiales.")
                 .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                 .Must(InputValidationHelpers.IsSafe)
                 .WithMessage("La descripción contiene caracteres peligrosos.")
                 .WithErrorCode(ErrorCodes.ValidationIllegalChar);
        }
    }
}
