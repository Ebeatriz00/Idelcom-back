using Application.DTOs.Account;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Account
{
    public class GetAccountById
    {
        private readonly IAccountRepository _repository;
        private readonly IMapper _mapper;

        public GetAccountById(IAccountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AccountByIdDto> ExecuteAsync(long accountId)
        {
            var entity = await _repository.GetByIdAsync(accountId);
            return _mapper.Map<AccountByIdDto>(entity);
        }
    }
}
