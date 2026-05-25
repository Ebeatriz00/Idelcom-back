using Application.DTOs.SomaDocumentType;
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
    public class SsomaDocumentTypeProfile : Profile
    {
        public SsomaDocumentTypeProfile()
        {
            CreateMap<SsomaDocumentTypeCreateDto, SsomaDocumentType>();
            CreateMap<SsomaDocumentTypeUpdateDto, SsomaDocumentType>();
            CreateMap<SsomaDocumentType, SsomaDocumentTypeResponseDto>();
            CreateMap<SsomaDocumentType, SsomaDocumentTypeSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
