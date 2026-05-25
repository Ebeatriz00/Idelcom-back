using Application.DTOs.CostCenters;
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

namespace Application.UseCases.CostCenters
{
    public class UpdateCostCenters
    {
        private readonly ICostCentersRepository _repository;
        private readonly IValidator<CostCentersUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateCostCenters(ICostCentersRepository repository, IValidator<CostCentersUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(CostCentersUpdateDto dto)
        {
            // 1. Validar la entrada
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();

                throw new AppValidationException(errores);
            }

            // 2. Validar que no exista otro registro con la misma descripción (excluyendo el actual)
            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.CostCentersId))
            {
                throw new DuplicateEntryException("El centro de costo ya existe para este negocio.");
            }

            // 3. Mapear el DTO a la entidad y ejecutar la actualización
            var entity = _mapper.Map<Core.Entities.CostCenters>(dto);
            var updated = await _repository.UpdateAsync(entity);

            // 4. Devolver la respuesta
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Centro de costo actualizado correctamente."
                : "No se pudo actualizar el centro de costo."
            };
        }
    }
}
