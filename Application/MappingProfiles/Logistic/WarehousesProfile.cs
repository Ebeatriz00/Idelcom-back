using Application.DTOs.Warehouses;
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
    public class WarehousesProfile : Profile
    {
        public WarehousesProfile()
        {
            CreateMap<WarehousesCreateDto, Warehouses>();
            CreateMap<WarehousesUpdateDto, Warehouses>();
            CreateMap<WarehousesItem, WarehousesResponseDto>();
            CreateMap<Warehouses, WarehousesByIdDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
