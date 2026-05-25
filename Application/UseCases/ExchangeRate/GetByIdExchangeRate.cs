using Application.DTOs.ExchangeRate;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ExchangeRate
{
    public class GetByIdExchangeRate
    {
        private readonly IExchangeRateRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdExchangeRate(IExchangeRateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ExchangeRateResponseDto> ExecuteAsync(long exchangeRateId)
        {
            var entity = await _repository.GetByIdAsync(exchangeRateId);
            return _mapper.Map<ExchangeRateResponseDto>(entity);
        }
    }
}
