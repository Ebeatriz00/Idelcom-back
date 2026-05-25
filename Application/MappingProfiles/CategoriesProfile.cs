using Application.DTOs.Categories;
using AutoMapper;
using Core.Entities.Logistic;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class CategoriesProfile : Profile
    {
        public CategoriesProfile()
        {
            CreateMap<CategoriesCreateDto, Categories>();
            CreateMap<CategoriesUpdateDto, Categories>();
            CreateMap<Categories, CategoriesResponseDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
