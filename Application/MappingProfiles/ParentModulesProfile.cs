using Application.DTOs.ParentModules;
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
    public class ParentModulesProfile : Profile
    {
        public ParentModulesProfile() {
            CreateMap<ParentModulesCreateDto, ParentModules>();
            CreateMap<ParentModulesUpdateDto, ParentModules>();
            CreateMap<ParentModules, ParentModulesResponseDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
