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
    public class ClientsActivityCreateValidator : AbstractValidator<ClientActivityCreateDto>
    {
        public ClientsActivityCreateValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ClientsId)
                .GreaterThan(0).WithMessage("El cliente es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
