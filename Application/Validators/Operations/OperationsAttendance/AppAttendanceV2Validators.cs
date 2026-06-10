using Application.DTOs.Operations.OperationsAttendance;
using FluentValidation;

namespace Application.Validators.Operations.OperationsAttendance
{
    public class AppAttendanceV2CreateValidator : AbstractValidator<AppAttendanceV2CreateDto>
    {
        public AppAttendanceV2CreateValidator()
        {
            RuleFor(x => x.AttendanceDate)
                .NotEmpty()
                .Must(BeLocalTime)
                .WithMessage("La fecha de asistencia debe estar en formato local.");

            RuleFor(x => x.OperationsId).GreaterThan(0);

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

            RuleForEach(x => x.Details).SetValidator(x => new AppAttendanceV2DetailValidator(x.SessionType));
        }

        private bool BeLocalTime(DateTime date) => date.Kind != DateTimeKind.Utc;
    }

    public class AppAttendanceV2DetailValidator : AbstractValidator<AppAttendanceBatchV2DetailDto>
    {
        public AppAttendanceV2DetailValidator(string sessionType)
        {
            RuleFor(x => x.WorkerId).GreaterThan(0);
            RuleFor(x => x.AttendanceStatusId).GreaterThan(0);

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

    public class SyncAppAttendanceBatchV2Validator : AbstractValidator<AppAttendanceV2SyncDto>
    {
        public SyncAppAttendanceBatchV2Validator()
        {
            RuleFor(x => x.AttendanceDate)
                .NotEmpty()
                .Must(BeLocalTime)
                .WithMessage("La fecha de asistencia debe estar en formato local.");

            RuleFor(x => x.OperationsId).GreaterThan(0);

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

            RuleForEach(x => x.Details).SetValidator(new SyncAppAttendanceV2DetailValidator());
        }

        private bool BeLocalTime(DateTime date) => date.Kind != DateTimeKind.Utc;
    }

    public class SyncAppAttendanceV2DetailValidator : AbstractValidator<AppAttendanceBatchDetailV2SyncDto>
    {
        public SyncAppAttendanceV2DetailValidator()
        {
            RuleFor(x => x.WorkerId).GreaterThan(0);
            RuleFor(x => x.AttendanceStatusId).GreaterThan(0);

            RuleFor(x => x.Observation)
                .NotEmpty()
                .When(x => x.AttendanceStatusId == 2)
                .WithMessage("La observación es obligatoria cuando el estado es 'NO ASISTIO'.");
        }
    }
}
