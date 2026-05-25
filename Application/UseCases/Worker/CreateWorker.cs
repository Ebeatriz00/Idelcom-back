using Application.DTOs.Worker;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Worker
{
    public class CreateWorker
    {
        private readonly IWorkerRepository _repository;
        private readonly IValidator<WorkerCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateWorker(IWorkerRepository repository, IValidator<WorkerCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(WorkerCreateDto dto)
        {
            // 1. Validación con FluentValidation
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                            .ToList();
                throw new AppValidationException(errores);
            }

            // 2. Verificar duplicidad (ejemplo con documento)
            var yaExiste = await _repository.ExistsAsync(dto.WorkerDocument, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El trabajador ya existe para este negocio.");

            // 3. Mapear DTO → Entidad
            var entity = _mapper.Map<Core.Entities.Worker>(dto);

            // 4. Guardar
            await _repository.AddAsync(entity);

            // 5. Respuesta global
            return new GlobalResponse
            {
                Status = 1,
                Message = "Trabajador creado exitosamente.",
            };
        }
    }

}
