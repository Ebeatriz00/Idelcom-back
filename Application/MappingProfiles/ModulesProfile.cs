using Application.DTOs.Modules;
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
    public class ModulesProfile : Profile
    {
        public ModulesProfile() {
            CreateMap<ModulesCreateDto, Modules>();
            CreateMap<ModulesUpdateDto, Modules>();
            CreateMap<Modules, ModulesResponseDto>();
            CreateMap<Modules, ModulesSelectDto>();
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
