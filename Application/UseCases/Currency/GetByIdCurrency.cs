using Application.DTOs.Currency;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Currency
{
    public class GetByIdCurrency
    {
        private readonly ICurrencyRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdCurrency(ICurrencyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<CurrencyResponseDto> ExecuteAsync(long currencyId)
        {
            var entities = await _repository.GetByIdAsync(currencyId);
            return _mapper.Map<CurrencyResponseDto>(entities);
        }
    }
}
