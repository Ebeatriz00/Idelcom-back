using Application.DTOs.DocumentType;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;

namespace Application.MappingProfiles;

public class DocumentTypeProfile : Profile
{
    public DocumentTypeProfile()
    {
        CreateMap<DocumentTypeCreateDto, DocumentType>();
        CreateMap<DocumentTypeUpdateDto, DocumentType>();
        CreateMap<DocumentType, DocumentTypeSelectDto>();
        CreateMap<DocumentType, DocumentTypeResponseDto>();
        CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
    }
}
