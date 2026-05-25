using Application.DTOs.Operations.OperationsAttendance;
using FluentValidation;

namespace Application.Validators.Operations.OperationsAttendance
{
    public class AppAttendanceCreateValidator : AbstractValidator<AppAttendanceCreateDto>
    {
        public AppAttendanceCreateValidator()
        {
            RuleFor(x => x.AttendanceDate)
                .NotEmpty()
                .Must(BeLocalTime)
                .WithMessage("La fecha de asistencia debe estar en formato local.");

            RuleFor(x => x.WorkOrderId).GreaterThan(0);
            RuleFor(x => x.SquadId).GreaterThan(0);

            RuleFor(x => x.SessionType)
                .Must(x => x == "ENTRADA" || x == "SALIDA")
                .WithMessage("El tipo de sesión debe ser 'ENTRADA' o 'SALIDA'.");

            RuleFor(x => x.SessionStartTime)
                .NotEmpty()
                .Must(BeLocalTime)
                .WithMessage("La hora de inicio debe estar en formato local (sin 'Z' ni desfase UTC).");

            RuleFor(x => x.SessionEndTime)
                .NotEmpty()
                .Must(BeLocalTime)
                .WithMessage("La hora de fin debe estar en formato local (sin 'Z' ni desfase UTC).")
                .GreaterThanOrEqualTo(x => x.SessionStartTime)
                .WithMessage("La hora de fin no puede ser menor a la de inicio.");

            RuleForEach(x => x.Details).SetValidator(x => new AppAttendanceDetailValidator(x.SessionType));
        }

        private bool BeLocalTime(DateTime date)
        {
            // DateTimeKind.Utc indica que se recibió con 'Z' o offset UTC
            return date.Kind != DateTimeKind.Utc;
        }
    }

    public class AppAttendanceDetailValidator : AbstractValidator<AppAttendanceBatchDetailDto>
    {
        public AppAttendanceDetailValidator(string sessionType)
        {
            RuleFor(x => x.AssignmentId).GreaterThan(0);
            RuleFor(x => x.WorkerId).GreaterThan(0);
            RuleFor(x => x.AttendanceStatusId).GreaterThan(0);

            // Regla: Si no asistió, la observación es obligatoria (2 = NO ASISTIO)
            RuleFor(x => x.Observation)
                .NotEmpty()
                .When(x => x.AttendanceStatusId == 2)
                .WithMessage("La observación es obligatoria cuando el estado es 'NO ASISTIO'.");

            RuleFor(x => x.CheckTime)
                .NotEmpty()
                .Must(x => x.Kind != DateTimeKind.Utc)
                .WithMessage("La hora de registro (CheckTime) debe estar en formato local (sin 'Z').");
        }
    }
}
