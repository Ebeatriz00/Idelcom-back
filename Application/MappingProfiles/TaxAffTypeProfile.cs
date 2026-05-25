using Application.DTOs.TaxAffType;
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
    public class TaxAffTypeProfile : Profile
    { 
        public TaxAffTypeProfile() 
        {
            CreateMap<TaxAffTypeCreateDto, TaxAffType>();
            CreateMap<TaxAffTypeUpdateDto, TaxAffType>();
            CreateMap<TaxAffType, TaxAffTypeResponseDto>();
            CreateMap<TaxAffType, TaxAffTypeSelectDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
