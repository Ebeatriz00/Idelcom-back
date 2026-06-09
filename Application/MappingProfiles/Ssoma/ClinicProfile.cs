using Application.DTOs.Ssoma.Clinics;
using AutoMapper;
using Core.Entities.Ssoma.Clinics;
using Core.Projections.Ssoma.Clinics;

namespace Application.MappingProfiles.Ssoma
{
    public class ClinicProfile : Profile
    {
        public ClinicProfile()
        {
            CreateMap<Clinic, ClinicResponseDto>();
            CreateMap<ClinicSelectItem, ClinicSelectDto>();
        }
    }
}
