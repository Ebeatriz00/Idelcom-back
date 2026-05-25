using Application.DTOs.OperationsTeamSsoma;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.OperationsTeamSsoma
{
    public class OperationsTeamSsomaUpdateValidator : AbstractValidator<OperationsTeamSsomaUpdateDto>
    {
        public OperationsTeamSsomaUpdateValidator()
        {
            
            RuleFor(x => x.BusinessId)
                .GreaterThan(0)
                .WithMessage("La empresa es obligatoria.");

            RuleFor(x => x.SsomaProcessId)
                .GreaterThan(0)
                .WithMessage("El proceso SSOMA es obligatorio.");

            RuleFor(x => x.UpdateUser)
                .GreaterThan(0)
                .WithMessage("El usuario actualizador es obligatorio.");

            RuleFor(x => x.TeamSsoma)
                .NotEmpty()
                .WithMessage("Debe agregar al menos un miembro SSOMA.");

            RuleForEach(x => x.TeamSsoma)
                .SetValidator(new OperationsTeamSsomaAssignmentUpdateValidator());

            // SE ELIMINÓ LA RESTRICCIÓN DE UN SOLO RESPONSABLE PRINCIPAL
            // PARA EVITAR BLOQUEOS EN EL FLUJO DE REGISTRO.

            RuleFor(x => x.TeamSsoma)
                .Must(list =>
                    list.Where(t => t.WorkerId.HasValue)
                        .Select(t => t.WorkerId!.Value)
                        .Distinct()
                        .Count()
                    == list.Count(t => t.WorkerId.HasValue))
                .WithMessage("No se pueden repetir trabajadores.");
        }
    }

    public class OperationsTeamSsomaAssignmentUpdateValidator : AbstractValidator<OperationsTeamSsomaAssignmentUpdateDto>
    {
        public OperationsTeamSsomaAssignmentUpdateValidator()
        {
            RuleFor(x => x.WorkerId)
                .NotNull()
                .WithMessage("El trabajador es obligatorio.");

            RuleFor(x => x.SsomaRoleId)
                .GreaterThan(0)
                .WithMessage("El rol SSOMA es obligatorio.");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("La fecha de inicio es obligatoria.");
        }
    }
}
