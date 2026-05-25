using Application.DTOs.ProfilesPermissions;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class ProfilesPermissionsProfile : Profile
    {
        public ProfilesPermissionsProfile() {

            CreateMap<ProfilesPermissionsCreateDto, ProfilesPermissions>();
            CreateMap<ProfilesPermissionsUpdateDto, ProfilesPermissions>();
            CreateMap<ProfilesPermissions, ProfilesPermissionsResponseDto>();
            CreateMap<ProfilesPermissions, ProfilesPermissionsByIdDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
