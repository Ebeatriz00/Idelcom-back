using Application.DTOs.LeadsStatus;
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


namespace Application.UseCases.LeadsStatus
{
    public class UpdateLeadsStatus
    {
        private readonly ILeadsStatusRepository _repository;
        private readonly IValidator<LeadsStatusUpdateDto> _validator;
        private readonly IMapper _mapper;
        public UpdateLeadsStatus(ILeadsStatusRepository repository, IValidator<LeadsStatusUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(LeadsStatusUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.LeadsStatusId))
                throw new Application.Exceptions.DuplicateEntryException("El estado de lead ya existe para este negocio.");
            
            var entity = _mapper.Map<Core.Entities.LeadsStatus>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Estado de lead actualizado correctamente." : "No se pudo actualizar el estado de lead."
            };
        }
    }
}
