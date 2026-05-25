using Application.DTOs.Area;
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


namespace Application.UseCases.Area
{
    public class UpdateArea
    {
        private readonly IAreaRepository _repository;
        private readonly IValidator<AreaUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateArea(IAreaRepository repository, IValidator<AreaUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(AreaUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.AreaId))
            {
                throw new Exceptions.DuplicateEntryException("El área ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.Area>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Área actualizada correctamente."
                    : "Error al actualizar el área."
            };
        }
    }

}
