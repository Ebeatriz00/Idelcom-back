using Application.DTOs.ProcessType;
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

namespace Application.UseCases.ProcessType
{
    public class UpdateProcessType
    {
        private readonly IProcessTypeReporsitory _repository;
        private readonly IValidator<ProcessTypeUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateProcessType(IProcessTypeReporsitory repository, IValidator<ProcessTypeUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProcessTypeUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.DescType, dto.BusinessId, dto.ProcessTypeId))
            {
                throw new DuplicateEntryException("El tipo de proceso ya existe para este negocio.");
            }
            var entity = _mapper.Map<Core.Entities.ProcessType>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Actualizado correctamente."
                : "Error al actualizar."
            };
        }
    }
}
