using Application.DTOs.PreSaleProyects;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.PreSaleProyects
{
    public class PreSaleProyectsUpdateValidator : AbstractValidator<PreSaleProyectsUpdateDto>
    {
        public PreSaleProyectsUpdateValidator()
        {

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario que actualiza es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ClientsId)
                .GreaterThan(0).WithMessage("El cliente es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ContactsCrmId)
                .GreaterThan(0).WithMessage("El contacto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.OpportunityId)
                .GreaterThan(0).WithMessage("La oportunidad es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.StatePreSaleId)
                .GreaterThan(0).WithMessage("El estado de pre-venta es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(100).WithMessage("La descripción no debe exceder los 100 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()\/]+$")
                .WithMessage("La descripción contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("La fecha de inicio es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.EndDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La fecha de fin es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("La fecha de fin no puede ser anterior a la fecha de inicio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.ResponsibleId)
                .GreaterThanOrEqualTo(0).WithMessage("ID de responsable inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.SupervisorId)
                .GreaterThanOrEqualTo(0).WithMessage("ID de supervisor inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.SsomaId)
                .GreaterThanOrEqualTo(0).WithMessage("ID de SSOMA inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.TecLeaderId)
                .GreaterThanOrEqualTo(0).WithMessage("ID de líder técnico inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.QuotationNumberId)
                .GreaterThanOrEqualTo(0).WithMessage("ID de cotización inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.OrderNumberId)
                .GreaterThanOrEqualTo(0).WithMessage("ID de orden inválido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.OrderDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de orden no puede ser futura.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .When(x => x.OrderDate != default(DateTime));
        }
    }               
}
