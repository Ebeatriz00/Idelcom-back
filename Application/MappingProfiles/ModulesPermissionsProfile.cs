using Application.DTOs.Business;
using Application.DTOs.ModulePermission;
using Application.DTOs.ModulesPermissions;
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
    public class ModulePermissionProfile : Profile
    {
            public ModulePermissionProfile()
            {
                CreateMap<ModulesPermissionsCreateDto, ModulesPermissions>();
                CreateMap<ModulesPermissionsUpdateDto, ModulesPermissions>();
                CreateMap<ListPermissions, ListPermissionsDto>();
                CreateMap<ModulesPermissions, ModulesPermissionsResponseDto>()
                        .ForMember(
                            d => d.ListModulesPermissions,
                            opt => opt.MapFrom(s => s.ListModulesPermissions ?? new List<ListPermissions>())
                        );

                CreateMap<ModulesPermissions, ModulesPermissionsByIdDto>();
                    CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            }
        }
    }
    

