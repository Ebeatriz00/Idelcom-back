using Application.DTOs.StateOpportunity;
using Application.DTOs.StatePreSale;
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
    public class StatePreSaleProfile : Profile
    {
        public StatePreSaleProfile()
        {

            CreateMap<StatePreSaleCreateDto, StatePreSale>();
            CreateMap<StatePreSaleUpdateDto, StatePreSale>();
            CreateMap<StatePreSale, StatePreSaleResponseDto>();
            CreateMap<StatePreSale, StatePreSaleSelectDto>();
            CreateMap<StatePreSale, StatePreSaleByIdDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));

        }
    }
}
