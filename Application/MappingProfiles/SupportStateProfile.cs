using Application.DTOs.SupportState;
using AutoMapper;
using Core.Entities;
using Core.Projections;

namespace Application.MappingProfiles
{
    public class SupportStateProfile : Profile
    {
        public SupportStateProfile()
        {
            CreateMap<SupportStateSelectItem, SupportStateSelectDto>();
            CreateMap<SupportState, SupportStateGetByIdDto>();
        }
    }
}
