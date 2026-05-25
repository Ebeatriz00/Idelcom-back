using Application.DTOs.ClientsActivity;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ClientsActivity
{
    public class ClientsActivityDeleteValidator : AbstractValidator<ClientsActivityDeleteDto>
    {
        public ClientsActivityDeleteValidator()
        {
            RuleFor(x => x.BusinessId)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("El negocio es obligatorio.")
                 .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.ClientsActivityId)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("El id de la actividad es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty);
        }
    }
}
