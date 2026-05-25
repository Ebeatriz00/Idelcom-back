using Application.DTOs.Warehouses;
using Core.Interfaces.Logistic;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Warehouses
{
    public class PatchWarehousesStatus
    {
        private readonly IWarehousesRepository _repository;
        private readonly IValidator<WarehousesStatusToggleDto> _validator;

        public PatchWarehousesStatus(IWarehousesRepository repository, IValidator<WarehousesStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(WarehousesStatusToggleDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.WarehousesId, dto.Status, userId, businessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del almacén actualizado correctamente."
                    : "No se pudo actualizar el estado del almacén."
            };
        }
    }
}
