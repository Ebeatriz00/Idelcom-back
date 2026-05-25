using Application.DTOs.Brands;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Brands
{
    public class GetAllBrands
    {
        private readonly IBrandsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllBrands(IBrandsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<BrandsResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<BrandsResponseDto>>(entities);
        }
    }
}
