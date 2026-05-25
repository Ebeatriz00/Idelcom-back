using Application.DTOs.ProductLines;
using Core.Interfaces.logistic;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;
namespace Application.UseCases.ProductLines
{
    public class PatchProductLinesStatus
    {
        private readonly IProductLinesRepository _repository;
        private readonly IValidator<ProductLinesStatusToggleDto> _validator;

        public PatchProductLinesStatus(IProductLinesRepository repository, IValidator<ProductLinesStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProductLinesStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.ProductLinesId, dto.Status, 3, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Estado de la línea de producto actualizado correctamente." : "No se pudo actualizar el estado de la línea de producto."
            };
        }
    }
}
