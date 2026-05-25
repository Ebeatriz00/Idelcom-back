using Application.DTOs.ProcessType;
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
    public class ProcessTypeProfile : Profile
    {
        public ProcessTypeProfile() {
            CreateMap<ProcessTypeCreateDto, ProcessType>();
            CreateMap<ProcessTypeUpdateDto, ProcessType>();
            CreateMap<ProcessType, ProcessTypeResponseDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
