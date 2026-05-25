using Application.DTOs.SsomaRequirement;
using AutoMapper;
using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Projections.SsomRequirement;

namespace Application.MappingProfiles.Ssoma
{
    public class SsomaRequirementProfile : Profile
    {
        public SsomaRequirementProfile()
        {
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap<SsomaRequirementCreateDto, SsomaRequirement>();
            CreateMap<SsomaRequirementUpdateDto, SsomaRequirement>();
            CreateMap<SsomaRequirement, SsomaRequirementByIdDto>();
            CreateMap<SsomaRequirement, SsomaRequirementResponseDto>();
            CreateMap<SsomaRequirementItem, SsomaRequirementResponseDto>();
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
