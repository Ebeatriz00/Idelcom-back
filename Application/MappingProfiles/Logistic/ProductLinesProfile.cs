using Application.DTOs.ProductLines;
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
    public class ProductLinesProfile : Profile
    {
        public ProductLinesProfile()
        {
            CreateMap<ProductLinesCreateDto, ProductLines>();
            CreateMap<ProductLinesUpdateDto, ProductLines>();
            CreateMap<ProductLines, ProductLinesResponseDto>();
            CreateMap<ProductLines, ProductLinesByIdDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
