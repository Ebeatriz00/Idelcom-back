using Application.DTOs.SsomaProcess;
using FluentValidation;

namespace Application.Validators.SsomaProcess
{
    public class SsomaProcessUpdateValidator : AbstractValidator<SsomaProcessUpdateDto>
    {
        public SsomaProcessUpdateValidator()
        {
            RuleFor(x => x.SsomaProcessId)
                .GreaterThan(0)
                .WithMessage("El proceso SSOMA es obligatorio.");

            RuleFor(x => x.OperationsId)
                .GreaterThan(0)
                .WithMessage("La operación es obligatoria.");

            RuleFor(x => x.CurrentStatusId)
                .GreaterThan(0)
                .WithMessage("El estado actual es obligatorio.");

        }
    }
}