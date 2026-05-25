using Application.DTOs.ClientsActivity;
using Core.Interfaces;
using FluentValidation;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.ClientsActivity
{
    public class UpdateActivityStatus
    {
        private readonly IClientsActivityRepository _repository;
        private readonly IValidator<ClientActivityUpdateDto> _validator;

        public UpdateActivityStatus(IClientsActivityRepository repository, IValidator<ClientActivityUpdateDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ClientActivityUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }
            var updated = await _repository.UpdateActivityStatusAsync(
                dto.BusinessId,
                dto.ClientsActivityId,
                dto.ActivityStateId,
                dto.UsersBy
            );

            // 4. Retorno de Respuesta
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado de la actividad actualizado correctamente."
                    : "No se pudo actualizar el estado (verifique permisos o existencia)."
            };
        }
    }
}
