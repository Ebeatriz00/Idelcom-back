using Application.DTOs.ProfilesPermissions;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProfilesPermissions
{
    public class GetByIdProfilesPermissions
    {
        private readonly IProfilesPermissionsRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdProfilesPermissions(IProfilesPermissionsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ProfilesPermissionsByIdDto> ExecuteAsync(long profilesPermissionsId)
        {
            var entity = await _repository.GetByIdAsync(profilesPermissionsId);
            return _mapper.Map<ProfilesPermissionsByIdDto>(entity);
        }
    }
}
