using Application.DTOs.Operations.Support;
using AutoMapper;
using Core.Entities.Operations;

namespace Application.MappingProfiles.Operations
{
    public class SupportProfile : Profile
    {
        public SupportProfile()
        {
            CreateMap<Core.Entities.Operations.Support, SupportResponseDto>();
            CreateMap<SupportCreateDto, Core.Entities.Operations.Support>();
            CreateMap<SupportUpdateDto, Core.Entities.Operations.Support>();
        }
    }
}
