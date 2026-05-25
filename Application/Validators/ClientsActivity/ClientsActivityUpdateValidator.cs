using Application.DTOs.ClientsActivity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ClientsActivity
{
    public class ClientsActivityUpdateValidator : AbstractValidator<ClientActivityUpdateDto>
    {
        public ClientsActivityUpdateValidator()
        {
            RuleFor(x => x.ClientsActivityId).GreaterThan(0).WithMessage("ID de actividad inválido.");
            RuleFor(x => x.ActivityStateId).GreaterThan(0).WithMessage("ID de estado inválido.");
            RuleFor(x => x.BusinessId).GreaterThan(0).WithMessage("Error de seguridad: BusinessId no encontrado.");
            RuleFor(x => x.UsersBy).GreaterThan(0).WithMessage("Error de seguridad: Usuario no identificado.");
        }
    }
}
