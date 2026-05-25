using Application.DTOs.Currency;
using Application.DTOs.Profiles;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Profiles
{
    public class GetByIdProfiles
    {
        private readonly IProfilesRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdProfiles(IProfilesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ProfilesResponseDto> ExecuteAsync(long profilesId)
        {
            var entities = await _repository.GetByIdAsync(profilesId);
            return _mapper.Map<ProfilesResponseDto>(entities);
        }
    }
}
