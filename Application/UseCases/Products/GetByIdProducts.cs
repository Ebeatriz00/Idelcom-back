using Application.DTOs.Products;
using AutoMapper;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Products
{
    public class GetByIdProducts
    {
        private readonly IProductsRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdProducts(IProductsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductsGetByIdDto> ExecuteAsync(long productsId)
        {
            var entity = await _repository.GetByIdAsync(productsId);
            return _mapper.Map<ProductsGetByIdDto>(entity);
        }
    }
}
