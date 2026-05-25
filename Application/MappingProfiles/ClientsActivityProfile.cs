using Application.DTOs.ClientsActivity;
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
    public class ClientsActivityProfile : Profile
    {
        public ClientsActivityProfile()
        {
            CreateMap<ClientActivityCreateDto, ClientsActivity>();
            CreateMap<ClientsActivity, ClientsActivityResponseDto>();
            CreateMap<ClientsActivityDeleteDto, ClientsActivity>();
            CreateMap<ClientActivityUpdateDto, ClientsActivity>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
