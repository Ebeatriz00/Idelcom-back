using Application.DTOs.Warehouses;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Warehouses
{
    public class GetAllWarehouses
    {
        private readonly IWarehousesRepository _repository;
        private readonly IMapper _mapper;

        public GetAllWarehouses(IWarehousesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<WarehousesResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<WarehousesResponseDto>>(entities);
        }
    }
}
