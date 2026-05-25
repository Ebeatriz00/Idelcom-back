using Application.DTOs.Profiles;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Profiles
{
    public class GetAllProfiles
    {
        private readonly IProfilesRepository _repository;
        private readonly IMapper _mapper;
        public GetAllProfiles(IProfilesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<ProfilesResponseDto>> ExecuteAsync(int businessId, string? search,int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search,page, pageSize);
            return _mapper.Map<PagedResult<ProfilesResponseDto>>(entities);
        }
    }
}