using Application.DTOs.SsomaHomologationPersonnel;
using AutoMapper;
using Core.Entities.Ssoma;
using Core.Entities.paginations;

namespace Application.MappingProfiles.Ssoma
{
    public class SsomaHomologationPersonnelProfile : Profile
    {
        public SsomaHomologationPersonnelProfile()
        {
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap<SsomaHomologationPersonnelCreateDto, SsomaHomologationPersonnel>();
            CreateMap<SsomaHomologationPersonnelUpdateDto, SsomaHomologationPersonnel>();
            CreateMap<SsomaHomologationPersonnel, SsomaHomologationPersonnelByIdDto>();
            CreateMap<SsomaHomologationPersonnel, SsomaHomologationPersonnelResponseDto>();
        }
    }
}
