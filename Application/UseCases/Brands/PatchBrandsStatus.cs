using Application.DTOs.Brands;
using Core.Interfaces.Logistic;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Brands
{
    public class PatchBrandsStatus
    {
        private readonly IBrandsRepository _repository;
        private readonly IValidator<BrandsStatusToggleDto> _validator;

        public PatchBrandsStatus(IBrandsRepository repository, IValidator<BrandsStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(BrandsStatusToggleDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.BrandsId, dto.Status, userId, businessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated 
                ? "Estado de la marca actualizado correctamente." 
                : "No se pudo actualizar el estado de la marca."
            };
        }
    }
}
