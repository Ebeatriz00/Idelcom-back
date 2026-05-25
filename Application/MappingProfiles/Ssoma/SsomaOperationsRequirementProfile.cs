using Application.DTOs.SsomaOperationsRequirement;
using AutoMapper;
using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Projections.SsomaOperationsRequirement;

namespace Application.MappingProfiles.Ssoma
{
    public class SsomaOperationsRequirementProfile : Profile
    {
        public SsomaOperationsRequirementProfile()
        {
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap<SsomaOperationsRequirementCreateDto, SsomaOperationsRequirement>();
            CreateMap<SsomaOperationsRequirementUpdateDto, SsomaOperationsRequirement>();
            CreateMap<SsomaOperationsRequirement, SsomaOperationsRequirementByIdDto>();
            CreateMap<SsomaOperationsRequirementItem, SsomaOperationsRequirementResponseDto>();
            CreateMap<SsomaOperationsRequirementByWorkerItem, SsomaOperationsRequirementByWorkerResponseDto>();
            CreateMap<ValidateSsomaOperationsRequirementByWorkerItem, ValidateSsomaOperationsRequirementByWorkerResponseDto>();
            CreateMap<SsomaOperationsRequirementMissingByWorkerItem, SsomaOperationsRequirementMissingByWorkerResponseDto>();
        }
    }
}
