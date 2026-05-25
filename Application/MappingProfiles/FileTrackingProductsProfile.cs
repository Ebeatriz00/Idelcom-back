using Application.DTOs.FileTrackingProducts;
using AutoMapper;
using Core.Entities;
using Core.Entities.Logistic;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class FileTrackingProductsProfile : Profile
    {
        public FileTrackingProductsProfile()
        {
            CreateMap<FileTrackingProductsCreateDto, FileTrackingProducts>()
                .ForMember(dest => dest.FileTitle, opt => opt.MapFrom(src => src.FileTitle));
            CreateMap<FileTrackingProducts, FileTrackingProductsResponseDto>()
                .ForMember(dest => dest.FileTitle, opt => opt.MapFrom(src => src.FileTitle));
            CreateMap<FileTrackingProductsDeleteDto, FileTrackingProducts>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
