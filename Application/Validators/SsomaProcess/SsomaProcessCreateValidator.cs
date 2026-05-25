using Application.DTOs.SsomaProcess;
using FluentValidation;

namespace Application.Validators.SsomaProcess
{
    public class SsomaProcessCreateValidator : AbstractValidator<SsomaProcessCreateDto>
    {
        public SsomaProcessCreateValidator()
        {
            // Operación
            RuleFor(x => x.OperationsId)
                .GreaterThan(0)
                .WithMessage("La operación es obligatoria.");

            // Estado
            RuleFor(x => x.CurrentStatusId)
                .GreaterThan(0)
                .WithMessage("El estado actual es obligatorio.");

            // SE ELIMINARON LAS VALIDACIONES DE FECHAS SEGÚN REQUERIMIENTO
            // PARA EVITAR PROBLEMAS CON LOS FLUJOS DE REGISTRO.
        }
    }
}