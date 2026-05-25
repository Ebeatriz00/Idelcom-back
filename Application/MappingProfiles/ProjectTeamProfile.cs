using Application.DTOs.PreSaleProyects;
using Application.DTOs.ProjectTeam;
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
    public class ProjectTeamProfile : Profile
    {
        public ProjectTeamProfile() {


            CreateMap<ProjectTeamCreateDto, ProjectTeam>();
            CreateMap<ProjectTeam, ProjectTeamResponseDto>();
            CreateMap<ProjectTeamDeleteDto, ProjectTeam>();


            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));

        }
    }
}
