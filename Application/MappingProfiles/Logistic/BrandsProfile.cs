using Application.DTOs.Brands;
using AutoMapper;
using Core.Entities.Logistic;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles.Logistic
{
    public class BrandsProfile : Profile
    {
        public BrandsProfile()
        {
            CreateMap<BrandsCreateDto, Brands>();
            CreateMap<BrandsUpdateDto, Brands>();
            CreateMap<Brands, BrandsResponseDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
