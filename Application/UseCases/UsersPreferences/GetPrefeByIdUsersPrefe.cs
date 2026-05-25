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
    public class GetPrefeByIdUsersPrefe
    {
        private readonly IUsersPreferencesRepository _repository;
        private readonly IMapper _mapper;
        public GetPrefeByIdUsersPrefe(IUsersPreferencesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<UsersPrefeDto> ExecuteAsync(long usersId, long businessId)
        {
            var entities = await _repository.GetPreferencesByIdAsync(usersId, businessId);
            return _mapper.Map<UsersPrefeDto>(entities);
        }
    }
}
