using Application.DTOs.Uom;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Uom
{
    public class PatchUomStatus
    {
        private readonly IUomRepository _repository;
        private readonly IValidator<UomStatusToggleDto> _validator;

        public PatchUomStatus(IUomRepository repository, IValidator<UomStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(UomStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.UomId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado de la unidad de medida actualizado correctamente."
                    : "No se pudo actualizar el estado de la unidad de medida."
            };
        }
    }
}
