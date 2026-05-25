using Application.DTOs.Currency;
using Application.DTOs.Paginations;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Currency
{
    public class GetAllCurrency
    {
        private readonly ICurrencyRepository _repository;
        private readonly IMapper _mapper;

        public GetAllCurrency(ICurrencyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<CurrencyResponseDto>> ExecuteAsync(int businessId,string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId,search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<CurrencyResponseDto>>(entities);
        }
    }
}
