using Application.DTOs.Hiring;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.DTOs.Hiring.HiringResponseDto;

namespace Application.MappingProfiles
{
    public class HiringProfile : Profile
    {
        public HiringProfile() {

            CreateMap<Hiring, HiringResponseDto>();
            CreateMap<HiringFile, HiringFileDto>();
            CreateMap<HiringUpdateStatusDto, Hiring>();
            CreateMap<MarkFileReadDto, FileTracking>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
