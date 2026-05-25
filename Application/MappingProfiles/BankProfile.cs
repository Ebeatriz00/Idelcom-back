
using Application.DTOs.Bank;
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
    public class BankProfile : Profile
    {
        public BankProfile()
        {
            CreateMap<BankCreateDto, Bank>();
            CreateMap<BankUpdateDto, Bank>();
            CreateMap<Bank, BankResponseDto>();
            CreateMap<Bank, BankSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
