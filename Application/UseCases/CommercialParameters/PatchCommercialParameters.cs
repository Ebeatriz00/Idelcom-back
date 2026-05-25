using Application.DTOs.CommercialParameters;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.CommercialParameters
{
    public class PatchCommercialParameters
    {
        private readonly ICommercialParametersRepository _repository;
        private readonly IValidator<CommercialParametersStatusToggleDto> _validator;
        public PatchCommercialParameters(ICommercialParametersRepository repository, IValidator<CommercialParametersStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }
        public async Task<GlobalResponse> ExecuteAsync(CommercialParametersStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }
            var updated = await _repository.PatchStatusAsync(dto.CommercialParametersId, dto.Status, dto.UsersBy, dto.BusinessId);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Parámetros comerciales actualizados correctamente."
                    : "No se pudieron actualizar los parámetros comerciales."
            };
        }
    }
}
