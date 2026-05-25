using Application.DTOs.Worker;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Worker
{
    public class WorkerCreateValidator : AbstractValidator<WorkerCreateDto>
    {
        public WorkerCreateValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.AreaId)
                .GreaterThan(0).WithMessage("El área es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.JobTitleId)
                .GreaterThan(0).WithMessage("El cargo es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.WorkerName)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(350).WithMessage("El nombre no debe exceder los 350 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("El nombre contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.WorkerLastName)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(350).WithMessage("El apellido no debe exceder los 350 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.WorkerDocument)
                .NotEmpty().WithMessage("El número de documento es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(15).WithMessage("El número de documento no debe exceder los 15 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("El correo no es válido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Phone)
                .Matches(@"^[0-9]{6,15}$").When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("El teléfono debe tener 9 dígitos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("El salario no puede ser negativo.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }

}
