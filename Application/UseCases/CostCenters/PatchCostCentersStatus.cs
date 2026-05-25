using Application.DTOs.CostCenters;
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
    public class PatchCostCentersStatus
    {
        private readonly ICostCentersRepository _repository;
        private readonly IValidator<CostCentersStatusToggleDto> _validator;

        public PatchCostCentersStatus(ICostCentersRepository repository, IValidator<CostCentersStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(CostCentersStatusToggleDto dto)
        {

            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();

                throw new AppValidationException(errores);
            }
            var updated = await _repository.PatchStatusAsync(dto.CostCentersId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado del centro de costo actualizado correctamente."
                : "No se pudo actualizar el estado del centro de costo."
            };
        }
    }
}
