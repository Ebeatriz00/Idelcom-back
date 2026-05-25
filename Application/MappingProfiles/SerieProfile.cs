using Application.DTOs.DocumentType;
using Application.DTOs.JobTitle;
using Application.DTOs.Series;
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
    public class SeriesProfile : Profile
    {
        public SeriesProfile()
        {
            CreateMap<SeriesCreateDto, Series>();
            CreateMap<SeriesUpdateDto, Series>();
            CreateMap<Series, SeriesResponseDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
