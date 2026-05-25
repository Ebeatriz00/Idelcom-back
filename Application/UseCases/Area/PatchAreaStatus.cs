using Application.DTOs.Area;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException =Application.Exceptions.ValidationException;

namespace Application.UseCases.Area
{
    public class PatchAreaStatus
    {
        private readonly IAreaRepository _repository;
        private readonly IValidator<AreaStatusToggleDto> _validator;

        public PatchAreaStatus(IAreaRepository repository, IValidator<AreaStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(AreaStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.AreaId, dto.Status, dto.UsersBy, dto.BusinessId);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del área actualizado correctamente."
                    : "No se pudo actualizar el estado del área."
            };
        }
    }
}

