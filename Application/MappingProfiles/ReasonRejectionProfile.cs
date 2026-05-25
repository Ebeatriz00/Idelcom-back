using Application.DTOs.ReasonRejection;
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
    public class ReasonRejectionProfile : Profile
    {
        public ReasonRejectionProfile()
        {
            CreateMap<ReasonRejectionCreateDto, ReasonRejection>();
            CreateMap<ReasonRejectionUpdateDto, ReasonRejection>();
            CreateMap<ReasonRejection, ReasonRejectionResponseDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
