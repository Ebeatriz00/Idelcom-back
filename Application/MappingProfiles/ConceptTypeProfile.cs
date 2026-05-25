using Application.DTOs.ConceptType;
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
    public class ConceptTypeProfile : Profile
    {
        public ConceptTypeProfile()
        {

            CreateMap<ConceptTypeCreateDto, ConceptType>();
            CreateMap<ConceptTypeUpdateDto, ConceptType>();
            CreateMap<ConceptType, ConceptTypeResponseDto>();
            CreateMap<ConceptType, ConceptTypeSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
