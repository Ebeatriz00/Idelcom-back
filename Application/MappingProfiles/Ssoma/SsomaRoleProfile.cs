using Application.DTOs.Ssoma;
using AutoMapper;
using Core.Projections.Ssoma;

namespace Application.MappingProfiles.Ssoma
{
    public class SsomaRoleProfile : Profile
    {
        public SsomaRoleProfile()
        {
            CreateMap<SsomaRoleSelectItem, SsomaRoleSelectDto>();
        }
    }
}
