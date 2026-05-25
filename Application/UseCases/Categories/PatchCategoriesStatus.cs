using Application.DTOs.Categories;
using Core.Interfaces.logistic;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Categories
{
    public class PatchCategoriesStatus
    {
        private readonly ICategoriesRepository _repository;
        private readonly IValidator<CategoriesStatusToggleDto> _validator;

        public PatchCategoriesStatus(ICategoriesRepository repository, IValidator<CategoriesStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(CategoriesStatusToggleDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.CategoriesId, dto.Status, userId, businessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated 
                ? "Estado de la categoría actualizado correctamente." 
                : "No se pudo actualizar el estado de la categoría."
            };
        }
    }
}
