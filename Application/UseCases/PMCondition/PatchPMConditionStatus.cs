using Application.DTOs.PMCondition;
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
    public class PatchPMConditionStatus
    {
        private readonly IPMConditionRepository _repository;
        private readonly IValidator<PMConditionStatusToggleDto> _validator;

        public PatchPMConditionStatus(IPMConditionRepository repository, IValidator<PMConditionStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(PMConditionStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.PMConditionId, dto.Status, dto.UsersBy, dto.BussinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado de la condición de pago actualizado correctamente."
                : "No se pudo actualizar el estado de la condición de pago."
            };
        }
    }
}
