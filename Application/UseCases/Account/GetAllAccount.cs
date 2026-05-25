using Application.DTOs.Account;
using Application.DTOs.Boxes;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Account
{
    public class GetAllAccount
    {
        private readonly IAccountRepository _repository;
        private readonly IMapper _mapper;

        public GetAllAccount(IAccountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<AccountResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<AccountResponseDto>>(entities);
        }
    }
}
