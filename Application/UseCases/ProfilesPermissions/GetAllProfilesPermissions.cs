using Application.DTOs.ProfilesPermissions;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProfilesPermissions
{
    public class GetAllProfilesPermissions
    {
        private readonly IProfilesPermissionsRepository _repository;
        private readonly IMapper _mapper;
        public GetAllProfilesPermissions(IProfilesPermissionsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Core.Entities.paginations.PagedResult<ProfilesPermissionsResponseDto>> ExecuteAsync(long profilesId, long businessId, int page, int pageSize, string? search = null)
        {
            var entities = await _repository.GetAllAsync(profilesId,businessId, page, pageSize, search);
            return _mapper.Map<PagedResult<ProfilesPermissionsResponseDto>>(entities);
        }
    }
}
