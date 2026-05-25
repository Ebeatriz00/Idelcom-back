using Application.DTOs.Orders;
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
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {
            CreateMap<Orders, OrdersResponseDto>();
            CreateMap<Orders, OrdersSsomaRegister>();
            CreateMap<Orders, QualitySupervisorCreateDto>();
            CreateMap<Orders, ProjectManagerCreateDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
