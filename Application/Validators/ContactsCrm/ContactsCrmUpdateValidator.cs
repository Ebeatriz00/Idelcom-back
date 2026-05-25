using Application.DTOs.ContactsCrm;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ContactsCrm
{
    public class ContactsCrmUpdateValidator : AbstractValidator<ContactsCrmUpdateDto>
    {
        public ContactsCrmUpdateValidator()
        {

            RuleFor(x => x.ContactsCrmId)
                .GreaterThan(0).WithMessage("ID de contacto inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);


            RuleFor(x => x.WorkerId)
                .GreaterThan(0).WithMessage("El trabajador es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ClientsId)
                .GreaterThan(0).WithMessage("El cliente es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.LeadsSourcesId)
                .GreaterThan(0).WithMessage("La fuente de lead es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ContactTypeId)
                .GreaterThan(0).WithMessage("El tipo de contacto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ContactName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de contacto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50).WithMessage("El nombre de contacto no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z\s\.áéíóúÁÉÍÓÚñÑ']+$")
                .WithMessage("El nombre de contacto contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.JobTitle)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El cargo es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50).WithMessage("El cargo no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("El cargo contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50).WithMessage("El email no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .EmailAddress().WithMessage("El formato del email es inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);


            RuleFor(x => x.Phone)
                .MaximumLength(15).WithMessage("El teléfono no debe exceder los 15 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[0-9\s+\-()]*$")
                .WithMessage("El formato del teléfono es inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Movil)
                .MaximumLength(15).WithMessage("El móvil no debe exceder los 15 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[0-9\s+\-()]*$")
                .WithMessage("El formato del móvil es inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
                .When(x => !string.IsNullOrEmpty(x.Movil));
        }
    }
}
