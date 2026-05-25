using Application.DTOs.OperationsTeamSsoma;
using FluentValidation;

namespace Application.Validators.OperationsTeamSsoma
{
    public class OperationsTeamSsomaCreateValidator : AbstractValidator<OperationsTeamSsomaCreateDto>
    {
        public OperationsTeamSsomaCreateValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0)
                .WithMessage("La empresa es obligatoria.");

            RuleFor(x => x.SsomaProcessId)
                .GreaterThan(0)
                .WithMessage("El proceso SSOMA es obligatorio.");

            RuleFor(x => x.CreateUser)
                .GreaterThan(0)
                .WithMessage("El usuario creador es obligatorio.");

            RuleFor(x => x.TeamSsoma)
                .NotEmpty()
                .WithMessage("Debe agregar al menos un miembro SSOMA.");

            RuleForEach(x => x.TeamSsoma)
                .SetValidator(new OperationsTeamSsomaAssignmentValidator());

            // SE ELIMINÓ LA RESTRICCIÓN DE UN SOLO RESPONSABLE PRINCIPAL
            // PARA EVITAR BLOQUEOS EN EL FLUJO DE REGISTRO.

            RuleFor(x => x.TeamSsoma)
                .Must(list =>
                    list.Where(t => t.WorkerId.HasValue)
                        .GroupBy(t => t.WorkerId!.Value)
                        .All(g => g.Count() == 1))
                .WithMessage("No se permite enviar trabajadores repetidos en el mismo grupo.");
        }

        public class OperationsTeamSsomaAssignmentValidator : AbstractValidator<OperationsTeamSsomaAssignmentDto>
        {
            public OperationsTeamSsomaAssignmentValidator()
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
}
