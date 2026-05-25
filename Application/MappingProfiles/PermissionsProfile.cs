using Application.DTOs.Permissions;
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
    public class PermissionsProfile : Profile
    {
        public PermissionsProfile()
        {
            CreateMap<PermissionsCreateDto, Permissions>();
            CreateMap<PermissionsUpdateDto, Permissions>();
            CreateMap<Permissions, PermissionsResponseDto>();
            CreateMap<Permissions, PermissionsSelectDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        
        }
    }
}
