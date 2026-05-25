using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Users
{
    public class GetExistsCodeUsers
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;

        public GetExistsCodeUsers(IUsersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> ExecuteAsync(string usersCode, long businessId)
        {
            return await _repository.GetLastUserCodeAsync(usersCode, businessId);
        }
    }
}
