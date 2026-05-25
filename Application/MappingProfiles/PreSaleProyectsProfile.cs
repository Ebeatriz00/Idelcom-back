using Application.DTOs.PreSaleProyects;
using Application.UseCases.PreSaleProyects;
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
    public class PreSaleProyectsProfile : Profile
    {
        public PreSaleProyectsProfile()
        {
            CreateMap<PreSaleProyectsCreateDto, PreSaleProyects>();
            CreateMap<PreSaleProyectsUpdateDto, PreSaleProyects>();
            CreateMap<PreSaleProyects, PreSaleProyectsResponseDto>();
            CreateMap<PreSaleProyects, PreSaleProyectsByIdDto>();
            CreateMap<PreSaleProyects, PreSaleProyectsSelectDto>();
            CreateMap<PreSaleProyects, PreSaleProyectsDetailDto>()
                .ForMember(dest => dest.ClientsName, opt => opt.MapFrom(src => src.ClientsDescription))
                .ForMember(dest => dest.ResponsibleDescription, opt => opt.MapFrom(src => src.ResponsibleDescription))
                .ForMember(dest => dest.CloseDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(src => src.ContactsCrmDescription));
            CreateMap<PreSaleProjectTask, PreSaleProjectTaskDto>();
            CreateMap<PreSaleProjectActivity, PreSaleProjectActivityDto>();
            CreateMap<ProjectFiletracking, ProjectFiletrackingDto>();
            CreateMap<HistoryPreSaleProjectsChanges,  HistoryPreSaleProjectsChangesDto>();
            CreateMap<ProjectFiletracking,  ProjectFiletrackingDto>();
            CreateMap<PreSaleProyects, PreSaleProyectsUpdateResponsibleDto>();
            CreateMap<PreSaleProjectsUpdateStateDto, PreSaleProyects>();


            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
