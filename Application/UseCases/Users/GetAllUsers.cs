using Application.DTOs.Users;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.UseCases.Users
{
    public class GetAllUsers
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;
        public GetAllUsers(IUsersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<UsersResponseDto>> ExecuteAsync(int businessId, string? search,int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search,page, pageSize);
            return _mapper.Map<PagedResult<UsersResponseDto>>(entities);
        }
    }
}
