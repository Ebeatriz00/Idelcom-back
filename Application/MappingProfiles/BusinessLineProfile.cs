using Application.DTOs.BusinessLine;
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
    public class BusinessLineProfile : Profile
    {
        public BusinessLineProfile()
        {

            CreateMap<BusinessLineCreateDto, BusinessLine>();
            CreateMap<BusinessLineUpdateDto, BusinessLine>();
            CreateMap<BusinessLine, BusinessLineResponseDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));

        }
    }
}
