using Application.DTOs.ContactType;
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
    public class ContactTypeProfile : Profile
    {
        public ContactTypeProfile()
        {
            CreateMap<ContactTypeCreateDto, ContactType>();
            CreateMap<ContactTypeUpdateDto, ContactType>();
            CreateMap<ContactType, ContactTypeResponseDto>();
            CreateMap<ContactType, ContactTypeSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
