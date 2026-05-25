using Application.DTOs.TaxAffType;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.TaxAffType
{
    public class GetAllTaxAffType
    {
        private readonly ITaxAffTypeRepository _repository;
        private readonly IMapper _mapper;
        public GetAllTaxAffType(ITaxAffTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<TaxAffTypeResponseDto>> ExecuteAsync(int businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<TaxAffTypeResponseDto>>(entities);
        }
    }
}
