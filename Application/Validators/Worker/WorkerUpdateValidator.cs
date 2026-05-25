using Application.DTOs.Worker;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Worker
{
    
    public class WorkerUpdateValidator : AbstractValidator<WorkerUpdateDto>
    {
        public WorkerUpdateValidator()
        {
            RuleFor(x => x.WorkerId)
                .GreaterThan(0).WithMessage("ID de trabajador inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.AreaId)
                .GreaterThan(0).WithMessage("El área es obligatoria.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.JobTitleId)
                .GreaterThan(0).WithMessage("El cargo es obligatorio.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.WorkerName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del trabajador es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(350).WithMessage("El nombre no debe exceder los 350 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[a-zA-Z\sáéíóúÁÉÍÓÚñÑ]+$")
                .WithMessage("El nombre contiene caracteres no permitidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El nombre contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.WorkerLastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido del trabajador es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(350).WithMessage("El apellido no debe exceder los 350 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[a-zA-Z\sáéíóúÁÉÍÓÚñÑ]+$")
                .WithMessage("El apellido contiene caracteres no permitidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("El apellido contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

            RuleFor(x => x.WorkerDocument)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El documento del trabajador es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(20).WithMessage("El documento no debe exceder los 20 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[a-zA-Z0-9]+$")
                .WithMessage("El documento contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("El correo electrónico no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Phone)
                .Cascade(CascadeMode.Stop)
                .Matches(@"^[0-9]{6,15}$").When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("El teléfono debe contener entre 9 dígitos numéricos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("El salario no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }

}
