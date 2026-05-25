using Application.DTOs.Operations.OperationsAttendance;
using FluentValidation;

namespace Application.Validators.Operations.OperationsAttendance
{
    public class AttendanceMatrixQueryValidator : AbstractValidator<AttendanceMatrixQueryDto>
    {
        public AttendanceMatrixQueryValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("El parámetro 'startDate' es obligatorio para consultar la matriz de asistencia.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("El parámetro 'endDate' es obligatorio para consultar la matriz de asistencia.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("La fecha de fin no puede ser menor a la fecha de inicio.");
        }
    }
}
