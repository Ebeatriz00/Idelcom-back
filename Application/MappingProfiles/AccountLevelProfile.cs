using Application.DTOs.AccountLevel;
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
    public class AccountLevelProfile : Profile
    {
        public AccountLevelProfile()
        {
            CreateMap<AccountLevel, AccountLevelSelectDto>();
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
