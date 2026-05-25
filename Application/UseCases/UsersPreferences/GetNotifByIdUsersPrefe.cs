using Application.DTOs.Users;
using Application.DTOs.UsersPreferences;
using AutoMapper;
using Core.Interfaces;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.UsersPreferences
{
    public class GetNotifByIdUsersPrefe
    {
        private readonly IUsersPreferencesRepository _repository;
        private readonly IMapper _mapper;

        public GetNotifByIdUsersPrefe(IUsersPreferencesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UsersPrefeNotifDto> ExecuteAsync(long usersId, long businessId)
        {
            var entities = await _repository.GetNotifByIdAsync(usersId, businessId);
            return _mapper.Map<UsersPrefeNotifDto>(entities);
        }
    }
}
