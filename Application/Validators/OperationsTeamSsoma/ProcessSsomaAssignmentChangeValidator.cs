using Application.DTOs.OperationsTeamSsoma;
using FluentValidation;

namespace Application.Validators.OperationsTeamSsoma
{
    public class ProcessSsomaAssignmentChangeValidator : AbstractValidator<ProcessSsomaAssignmentChangeDto>
    {
        public ProcessSsomaAssignmentChangeValidator()
        {
            RuleFor(x => x.ChangeType)
                .IsInEnum()
                .WithMessage("El tipo de cambio SSOMA es obligatorio.");

            RuleFor(x => x.BusinessId)
                .GreaterThan(0)
                .WithMessage("La empresa es obligatoria.");

            RuleFor(x => x.OperationsTeamSsomaId)
                .GreaterThan(0)
                .WithMessage("La asignación SSOMA es obligatoria.");

            RuleFor(x => x.SsomaProcessId)
                .GreaterThan(0)
                .WithMessage("El proceso SSOMA es obligatorio.");

            RuleFor(x => x.SsomaRoleId)
                .GreaterThan(0)
                .WithMessage("El rol SSOMA es obligatorio.");

            RuleFor(x => x.UpdateUser)
                .GreaterThan(0)
                .WithMessage("El usuario actualizador es obligatorio.");

            RuleFor(x => x.SssomaMovementTypeId)
                .GreaterThan(0)
                .WithMessage("El tipo de movimiento SSOMA es obligatorio.");

            RuleFor(x => x.MovementDate)
                .NotEmpty()
                .WithMessage("La fecha de movimiento es obligatoria.");

            RuleFor(x => x)
                .Must(x => x.MovementDate.Date >= x.StartDate.ToDateTime(TimeOnly.MinValue).Date)
                .WithMessage("La fecha de movimiento no puede ser menor que la fecha de inicio.");

            RuleFor(x => x)
                .Must(x => !x.EndDate.HasValue || x.EndDate >= x.StartDate)
                .WithMessage("La fecha de fin no puede ser menor que la fecha de inicio.");

            When(x => x.ChangeType == SsomaAssignmentChangeType.Update, () =>
            {
                RuleFor(x => x.WorkerId)
                    .Must(x => !x.HasValue || x.Value > 0)
                    .WithMessage("El trabajador informado no es válido.");

                RuleFor(x => x.ToSsomaProcessId)
                    .Must(x => !x.HasValue || x.Value > 0)
                    .WithMessage("El proceso destino informado no es válido.");
            });

            When(x => x.ChangeType == SsomaAssignmentChangeType.Relocation, () =>
            {
                RuleFor(x => x.ToSsomaProcessId)
                    .NotNull()
                    .WithMessage("El proceso destino es obligatorio para una reubicación.")
                    .GreaterThan(0)
                    .WithMessage("El proceso destino no es válido.");

                RuleFor(x => x.ReasonChange)
                    .NotEmpty()
                    .WithMessage("El motivo de cambio es obligatorio para una reubicación.");

                RuleFor(x => x)
                    .Must(x => x.FromSsomaProcessId.HasValue && x.ToSsomaProcessId.HasValue && x.FromSsomaProcessId.Value != x.ToSsomaProcessId.Value)
                    .WithMessage("El proceso destino debe ser distinto al proceso origen para una reubicación.");
            });

            When(x => x.ChangeType == SsomaAssignmentChangeType.Replacement, () =>
            {
                RuleFor(x => x.WorkerId)
                    .NotNull()
                    .WithMessage("El trabajador es obligatorio para un reemplazo.")
                    .GreaterThan(0)
                    .WithMessage("El trabajador no es válido.");

                RuleFor(x => x.ToSsomaProcessId)
                    .NotNull()
                    .WithMessage("El proceso destino es obligatorio para un reemplazo.")
                    .GreaterThan(0)
                    .WithMessage("El proceso destino no es válido.");

                RuleFor(x => x.ReasonChange)
                    .NotEmpty()
                    .WithMessage("El motivo de cambio es obligatorio para un reemplazo.");

                RuleFor(x => x.ReplacedAssignmentId)
                    .Must(x => !x.HasValue || x.Value > 0)
                    .WithMessage("La asignación reemplazada no es válida.");
            });
        }
    }
}
