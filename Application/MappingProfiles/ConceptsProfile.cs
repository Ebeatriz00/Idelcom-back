using Application.DTOs.Concepts;
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
    public class ConceptsProfile : Profile
    {
        public ConceptsProfile()
        {
            CreateMap<ConceptsCreateDto, Concepts>();
            CreateMap<ConceptsUpdateDto, Concepts>();
            CreateMap<Concepts, ConceptsResponseDto>();
            CreateMap<Concepts, ConceptsByIdDto>();
            CreateMap<Concepts, ConceptsSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
