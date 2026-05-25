using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    using Application.DTOs.Products;
    using AutoMapper;
    using Core.Entities.Logistic;
    using Core.Entities.paginations;
    using Core.Projections.Logistic;

    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<ProductsCreateDto, Products>()
                .ForMember(dest => dest.Files, opt => opt.Ignore());
            CreateMap<ProductsUpdateDto, Products>();
            CreateMap<ProductsItem, ProductsResponseDto>();
            CreateMap<Products, ProductsGetByIdDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
