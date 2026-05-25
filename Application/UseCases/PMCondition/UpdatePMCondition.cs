using Application.DTOs.PMCondition;
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


namespace Application.UseCases.PMCondition
{
    public class UpdatePMCondition
    {
        private readonly IPMConditionRepository _repository;
        private readonly IValidator<PMConditionUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdatePMCondition(IPMConditionRepository repository, IValidator<PMConditionUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(PMConditionUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.PMConditionId))
                throw new DuplicateEntryException("La condición de pago ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.PMCondition>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Condición de pago actualizada correctamente." : "Error al actualizar la condición de pago."
            };
        }
    }
}
