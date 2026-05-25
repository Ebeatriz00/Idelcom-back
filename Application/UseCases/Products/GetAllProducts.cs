using Application.DTOs.Products;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Products
{
    public class GetAllProducts
    {
        private readonly IProductsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllProducts(IProductsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductsResponseDto>> ExecuteAsync(long businessId, long? categoriesId, long? productTypeId, long? brandsId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, categoriesId, productTypeId, brandsId, search, page, pageSize);
            return _mapper.Map<PagedResult<ProductsResponseDto>>(entities);
        }
    }
}
