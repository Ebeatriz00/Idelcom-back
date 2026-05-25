using Application.DTOs.ActivityType;
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

namespace Application.UseCases.ActivityType
{
    public class UpdateActivityType
    {
        private readonly IActivityTypeRepository _repository;
        private readonly IValidator<ActivityTypeUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateActivityType(IActivityTypeRepository repository, IValidator<ActivityTypeUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ActivityTypeUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.ActivityDesc, dto.BusinessId, dto.LinkToken))
            {
                throw new DuplicateEntryException("el estado de actividades ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.ActivityType>(dto);
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
