using Application.DTOs.Bank;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Bank
{
    public class GetByIdBank
    {
        private readonly IBankRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdBank(IBankRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<BankResponseDto> ExecuteAsync(long bankId)
        {
            var entity = await _repository.GetByIdAsync(bankId);
            return _mapper.Map<BankResponseDto>(entity);
        }
    }
}
