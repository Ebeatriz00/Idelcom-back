using Application.DTOs.Clients;
using Application.Exceptions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.Clients
{
    public class UpdateClients
    {
        private readonly IClientsRepository _repository;
        private readonly IValidator<ClientsUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateClients(IClientsRepository repository, IValidator<ClientsUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ClientsUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            var (exists, sameSeller, dbWorkerId, vendedor, clienteDb, lastDesc, lastAt)
    = await _repository.ExistsAsync(dto.Documents, dto.BusinessId, dto.WorkerId, dto.ClientsId);

            if (exists)
            {
                string last = lastAt.HasValue
                    ? $" Última actividad: {lastDesc} ({lastAt:dd/MM/yyyy HH:mm})."
                    : "";

                if (sameSeller)
                {
                    throw new DuplicateEntryException(
                        $"Este cliente ya está registrado por ti como '{clienteDb}'.{last}"
                    );
                }

                if (dbWorkerId is null)
                {
                    throw new DuplicateEntryException(
                        $"Este cliente ya existe en el sistema y no tiene vendedor asignado (nombre: '{clienteDb}').{last}"
                    );
                }

                throw new DuplicateEntryException(
                    $"Este RUC ya fue registrado por el vendedor {vendedor} como '{clienteDb}'.{last}"
                );
            }

            var entity = _mapper.Map<Core.Entities.Clients>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Actualizado correctamente."
                    : "Error al actualizar el cliente."
            };
        }
    }
}
