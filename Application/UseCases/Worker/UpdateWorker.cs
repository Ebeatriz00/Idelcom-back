using Application.DTOs.Worker;
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
    public class UpdateWorker
    {
        private readonly IWorkerRepository _repository;
        private readonly IValidator<WorkerUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateWorker(IWorkerRepository repository, IValidator<WorkerUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(WorkerUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.WorkerDocument, dto.BusinessId, dto.WorkerId))
            {
                throw new Exceptions.DuplicateEntryException("El trabajador ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.Worker>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Trabajador actualizado correctamente."
                    : "Error al actualizar el trabajador."
            };
        }
    }

}
