using Application.DTOs.TaxAffType;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.TaxAffType
{
    public class PatchTaxAffType
    {
        private readonly ITaxAffTypeRepository _repository;
        private readonly IValidator<TaxAffTypeStatusToggleDto> _validator;
        public PatchTaxAffType(ITaxAffTypeRepository repository, IValidator<TaxAffTypeStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }
        public async Task<GlobalResponse> ExecuteAsync(TaxAffTypeStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            var updated = await _repository.PatchStatusAsync(dto.TaxAffTypeId, dto.Status, dto.UsersBy, dto.BusinessId);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado del tipo de afectación tributaria actualizado correctamente."
                : "No se pudo actualizar el estado del tipo de afectación tributaria."
            };
        }
    }
}
