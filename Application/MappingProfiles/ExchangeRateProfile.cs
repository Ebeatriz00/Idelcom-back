using Application.DTOs.ExchangeRate;
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
    public class ExchangeRateProfile : Profile
    {
        public ExchangeRateProfile()
        {
            CreateMap<ExchangeRateCreateDto, ExchangeRate>();
            CreateMap<ExchangeRateUpdateDto, ExchangeRate>();
            CreateMap<ExchangeRate, ExchangeRateResponseDto>();
            CreateMap<ExchangeRate, ExchangeRateSelectDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}