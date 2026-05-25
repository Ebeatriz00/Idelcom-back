using Application.DTOs.ProductTypes;
using Core.Interfaces.Logistic;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.ProductTypes
{
    public class PatchProductTypesStatus
    {
        private readonly IProductTypesRepository _repository;
        private readonly IValidator<ProductTypesStatusToggleDto> _validator;

        public PatchProductTypesStatus(IProductTypesRepository repository, IValidator<ProductTypesStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProductTypesStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.ProductTypesId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated 
                ? "Estado del tipo de producto actualizado correctamente." 
                : "No se pudo actualizar el estado del tipo de producto."
            };
        }
    }
}
