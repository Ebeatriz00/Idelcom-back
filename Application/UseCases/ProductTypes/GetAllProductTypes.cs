using Application.DTOs.ProductTypes;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProductTypes
{
    public class GetAllProductTypes
    {
        private readonly IProductTypesRepository _repository;
        private readonly IMapper _mapper;

        public GetAllProductTypes(IProductTypesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductTypesResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<ProductTypesResponseDto>>(entities);
        }
    }
}
