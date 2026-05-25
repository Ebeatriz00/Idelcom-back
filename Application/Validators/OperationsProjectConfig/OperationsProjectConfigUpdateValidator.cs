using Application.DTOs.OperationsProjectConfing;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.OperationsProjectConfig
{
    public class OperationsProjectConfigUpdateValidator : AbstractValidator<OperationsProjectConfigUpdateDto>
    {
        public OperationsProjectConfigUpdateValidator() {
            RuleFor(x => x.OperationsProjectConfigId)
                .GreaterThan(0);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0);

            RuleFor(x => x.OperationsId)
                .GreaterThan(0);

            RuleFor(x => x.EntryTime)
                .NotEmpty()
                .WithMessage("La hora de ingreso es obligatoria.");

            RuleFor(x => x.DepartureTime)
                .NotEmpty()
                .WithMessage("La hora de salida es obligatoria.");

            RuleFor(x => x.BeforeOfficialTime)
                .NotEmpty()
                .WithMessage("La hora límite previa al horario oficial es obligatoria.");

            RuleFor(x => x.MinutesTolerance)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(180);

            RuleFor(x => x.MinutesTolerance)
                .Equal(0)
                .When(x => !x.AllowDelay)
                .WithMessage("Si no se permite tardanza, la tolerancia debe ser 0.");

            RuleFor(x => x.IsRequireOvertimeApproval)
                .Equal(false)
                .When(x => x.IsRequireOvertime == false)
                .WithMessage("No puede requerirse aprobación de horas extra si no se requieren horas extra.");

        }
    }
}
