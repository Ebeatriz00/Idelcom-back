using Application.DTOs.ProductTypes;
using AutoMapper;
using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Projections.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles.Logistic
{
    public class ProductTypesProfile : Profile
    {
        public ProductTypesProfile()
        {
            CreateMap<ProductTypesCreateDto, ProductTypes>();
            CreateMap<ProductTypesUpdateDto, ProductTypes>();
            CreateMap<ProductTypes, ProductTypesResponseDto>();
            CreateMap<ProductTypes, ProductTypesByIdDto>();
            CreateMap<ProductTypes, ProductTypesSelectDto>();
            CreateMap<ProductTypeItem, ProductTypesResponseDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
