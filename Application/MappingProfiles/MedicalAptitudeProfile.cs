using Application.DTOs.MedicalAptitude;
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
    public class MedicalAptitudeProfile : Profile
    {
        public MedicalAptitudeProfile()
        {
            CreateMap<MedicalAptitudeCreateDto, MedicalAptitude>();
            CreateMap<MedicalAptitudeUpdateDto, MedicalAptitude>();
            CreateMap<MedicalAptitude, MedicalAptitudeResponseDto>();
            CreateMap<MedicalAptitude, MedicalAptitudeSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
