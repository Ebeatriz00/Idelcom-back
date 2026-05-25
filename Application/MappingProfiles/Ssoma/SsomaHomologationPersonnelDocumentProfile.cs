using Application.DTOs.SsomaHomologationPersonnelDocument;
using AutoMapper;
using Core.Entities.paginations;
using Core.Entities.Ssoma;

namespace Application.MappingProfiles.Ssoma
{
    public class SsomaHomologationPersonnelDocumentProfile : Profile
    {
        public SsomaHomologationPersonnelDocumentProfile()
        {
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap<SsomaHomologationPersonnelDocumentCreateDto, SsomaHomologationPersonnelDocument>();
            CreateMap<SsomaHomologationPersonnelDocumentUpdateDto, SsomaHomologationPersonnelDocument>();
            CreateMap<SsomaHomologationPersonnelDocument, SsomaHomologationPersonnelDocumentByIdDto>();
            CreateMap<SsomaHomologationPersonnelDocument, SsomaHomologationPersonnelDocumentResponseDto>();
        }
    }
}
