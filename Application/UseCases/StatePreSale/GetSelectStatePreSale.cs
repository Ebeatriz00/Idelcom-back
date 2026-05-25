using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.StatePreSale
{
    public class GetSelectStatePreSale
    {
        private readonly IStatePreSaleRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectStatePreSale(IStatePreSaleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var selectOptions = await _repository.GetForSelectAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(selectOptions);
        }
    }
}
