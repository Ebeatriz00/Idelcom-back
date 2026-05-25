using Application.DTOs.Business;
using AutoMapper;
using Core.Entities;

namespace Application.MappingProfiles
{
    public class BusinessProfile : Profile
    {
        public BusinessProfile()
        {
            CreateMap<BusinessCreateDto, Business>();
            CreateMap<AddressBusiness, AddressBusinessDto>();
            CreateMap<Business, BusinessViewDto>()
                .ForMember(d => d.AddressBusiness,
                           opt => opt.MapFrom(s => s.AddressBusiness ?? new List<AddressBusiness>()));
        }
    }
}
