using Application.DTOs.UsersPreferences;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.UsersPreferences
{
    public class GetSeattingByIdUsersPrefe
    {
        private readonly IUsersPreferencesRepository _repository;
        private readonly IMapper _mapper;

        public GetSeattingByIdUsersPrefe(IUsersPreferencesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UsersPrefeSettingDto> ExecuteAsync(long usersId, long businessId)
        {
            var entities = await _repository.GetSettingByIdAsync(usersId, businessId);
            return _mapper.Map<UsersPrefeSettingDto>(entities);
        }
    }
}
