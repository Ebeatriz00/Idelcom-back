using Application.DTOs.Suppliers;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Suppliers
{
    public class GetAllSuppliers
    {
        private readonly ISuppliersRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSuppliers(ISuppliersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<SuppliersResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<SuppliersResponseDto>>(entities);
        }
    }
}
