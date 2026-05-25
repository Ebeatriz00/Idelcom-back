using Application.DTOs.BusinessLine;
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


namespace Application.UseCases.BusinessLine
{
    public class UpdateBusinessLine
    {
        private readonly IBusinessLineRepository _repository;
        private readonly IValidator<BusinessLineUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateBusinessLine(IBusinessLineRepository repository, IValidator<BusinessLineUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(BusinessLineUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.DescLine, dto.BusinessId, dto.BusinessLineId))
            {
                throw new DuplicateEntryException("La linea de negocio ya existe para este negocio.");
            }
            var entity = _mapper.Map<Core.Entities.BusinessLine>(dto);
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
