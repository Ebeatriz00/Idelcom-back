using Application.DTOs.StatePreSale;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.StatePreSale
{
    public class GetStatePreSaleById
    {
        private readonly IStatePreSaleRepository _repository;
        private readonly IMapper _mapper;

        public GetStatePreSaleById(IStatePreSaleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StatePreSaleByIdDto> ExecuteAsync(long statePreSaleId)
        {
            var entity = await _repository.GetByIdAsync(statePreSaleId);
            return _mapper.Map<StatePreSaleByIdDto>(entity);
        }
    }
}
