using Application.DTOs.AccountType;
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
    public class AccountTypeProfile : Profile
    {
        public AccountTypeProfile()
        {
            CreateMap<AccountType, AccountTypeSelectDto>();
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
