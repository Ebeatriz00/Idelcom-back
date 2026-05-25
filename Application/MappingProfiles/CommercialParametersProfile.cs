using Application.DTOs.CommercialParameters;
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
    public class CommercialParametersProfile : Profile
    {
        public CommercialParametersProfile()
        {
            CreateMap<CreateCommercialParametersDto, CommercialParameters>();
            CreateMap<UpdateCommercialParametersDto, CommercialParameters>();
            CreateMap<CommercialParameters, ResponseCommercialParametersDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
