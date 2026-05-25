using Application.DTOs.PaymentType;
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
    public class PaymentTypeProfile : Profile
    {
        public PaymentTypeProfile() {

            CreateMap<PaymentTypeCreateDto, PaymentType>();
            CreateMap<PaymentTypeUpdateDto, PaymentType>();
            CreateMap<PaymentType, PaymentTypeResponseDto>();
            CreateMap<PaymentType, PaymentTypeSelectDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
