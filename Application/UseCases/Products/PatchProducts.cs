using Application.DTOs.Products;
using AutoMapper;
using Core.Interfaces.Logistic;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Products
{
    public class PatchProducts
    {
        private readonly IProductsRepository _repository;
        private readonly IValidator<ProductsStatusToggleDto> _validator;
        private readonly IMapper _mapper;

        public PatchProducts(IProductsRepository repository, IValidator<ProductsStatusToggleDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProductsStatusToggleDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();

                throw new AppValidationException(errores);

            }

            var updated = await _repository.PatchStatusAsync(dto.ProductsId, dto.Status, userId, businessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado actualizado correctamente."
                : "No se pudo actualizar el estado."
            };
        }
    }
}
