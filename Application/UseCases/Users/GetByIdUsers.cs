using Application.DTOs.Users;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Users
{
    public class GetByIdUsers
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdUsers(IUsersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<UsersResponseIdDto> ExecuteAsync(long usersId)
        {
            var entities = await _repository.GetByIdAsync(usersId);
            return _mapper.Map<UsersResponseIdDto>(entities);
        }
    }
}
