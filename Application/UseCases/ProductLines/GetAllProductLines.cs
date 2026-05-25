using Application.DTOs.ProductLines;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProductLines
{
    public class GetAllProductLines
    {
        private readonly IProductLinesRepository _repository;
        private readonly IMapper _mapper;

        public GetAllProductLines(IProductLinesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductLinesResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<ProductLinesResponseDto>>(entities);
        }
    }
}
