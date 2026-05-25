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
    public class GetByIdUsersSetting
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdUsersSetting(IUsersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<UsersSettingResponseIdDto> ExecuteAsync(long usersId)
        {
            var entities = await _repository.GetSettingByIdAsync(usersId);
            return _mapper.Map<UsersSettingResponseIdDto>(entities);
        }
    }
}
