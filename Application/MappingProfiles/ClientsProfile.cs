using Application.DTOs.Clients;
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
    public class ClientsProfile : Profile
    {
        public ClientsProfile()
        {
            CreateMap<ClientsCreateDto, Clients>();
            CreateMap<ClientsUpdateDto, Clients>();
            CreateMap<ClientsUpdateChangeSalesDto, Clients>();
            CreateMap<Clients, ClientsResponseDto>();
            CreateMap<Clients, ClientsHistoryResponseDto>();
            CreateMap<Clients, ClientsByIdDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
