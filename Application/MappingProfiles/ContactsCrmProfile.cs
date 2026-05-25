using Application.DTOs.ContactsCrm;
using Application.DTOs.SubTasks;
using Application.DTOs.SubTasks.Application.DTOs.SubTasks;
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
    public class ContactsCrmProfile : Profile
    {
        public ContactsCrmProfile()
        {
            CreateMap<ContactsCrmCreateDto, ContactsCrm>();
            CreateMap<ContactsCrmUpdateDto, ContactsCrm>();
            CreateMap<ContactsCrm, ContactsCrmResponseDto>();
            CreateMap<ContactsCrm, ContactsCrmByIdDto>();
            CreateMap<ContactsCrm, ContactsCrmSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}

