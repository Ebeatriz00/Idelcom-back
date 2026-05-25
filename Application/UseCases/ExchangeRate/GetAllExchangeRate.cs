using Application.DTOs.ExchangeRate;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ExchangeRate
{
    public class GetAllExchangeRate
    {
        private readonly IExchangeRateRepository _repository;
        private readonly IMapper _mapper;

        public GetAllExchangeRate(IExchangeRateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ExchangeRateResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<ExchangeRateResponseDto>>(entities);
        }
    }
}
