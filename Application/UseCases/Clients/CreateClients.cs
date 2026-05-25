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
    public class CreateClients
    {
        private readonly IClientsRepository _repository;
        private readonly IValidator<ClientsCreateDto> _validator;
        private readonly IMapper _mapper;


        public CreateClients(IClientsRepository repository, IValidator<ClientsCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ClientsCreateDto dto)
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
                = await _repository.ExistsAsync(dto.Documents, dto.BusinessId, dto.WorkerId);

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
                        $"Este cliente ya existe y aún no tiene vendedor asignado (nombre: '{clienteDb}').{last}"
                    );
                }

                throw new DuplicateEntryException(
                    $"Este RUC ya fue registrado por el vendedor {vendedor} como '{clienteDb}'.{last}"
                );
            }

            var entity = _mapper.Map<Core.Entities.Clients>(dto);

            var dbRes = await _repository.AddAsync(entity);
            if (dbRes.Status != 1)
            {
                return dbRes;
            }
            dbRes.Message = "Cliente creado exitosamente.";

            return dbRes;
        }
    }
}
