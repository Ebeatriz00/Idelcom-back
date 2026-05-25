using Application.DTOs.Clients;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Clients
{
    public class ClientsUpdateChangeSalesValidator : AbstractValidator<ClientsUpdateChangeSalesDto>
    {
        public ClientsUpdateChangeSalesValidator() {


            RuleFor(x => x.ClientsId)
                .GreaterThan(0).WithMessage("ID del cliente inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
