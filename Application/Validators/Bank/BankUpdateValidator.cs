using Application.DTOs.Bank;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Bank
{
    public class BankUpdateValidator : AbstractValidator<BankUpdateDto>
    {
        public BankUpdateValidator()
        {
            RuleFor(x => x.BankId) 
                .GreaterThan(0).WithMessage("ID del banco inválido.") 
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción del banco es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(60) 
                .WithMessage("La descripción no debe exceder los 60 caracteres.") 
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción tiene caracteres especiales.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("La descripción contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);

     
            RuleFor(x => x.Abrv)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("La abreviatura es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(5) 
                .WithMessage("La abreviatura no debe exceder los 5 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9]+$")
                .WithMessage("La abreviatura solo puede contener letras y números.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                .Must(InputValidationHelpers.IsSafe)
                .WithMessage("La abreviatura contiene caracteres peligrosos.")
                .WithErrorCode(ErrorCodes.ValidationIllegalChar);
        }
    }
}
