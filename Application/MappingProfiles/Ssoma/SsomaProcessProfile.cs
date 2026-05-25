using Application.DTOs.SsomaProcess;
using AutoMapper;
using Core.Entities.Ssoma;
using Core.Projections.Ssoma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles.Ssoma
{
    public class SsomaProcessProfile : Profile
    {
        public SsomaProcessProfile() {
            CreateMap<SsomaProcessListItem, SsomaProcessResponseDto>();
            CreateMap<SsomaProcess, SsomaProcessByIdDto>();
            CreateMap<SsomaProcessCreateDto, SsomaProcess>();
            CreateMap<SsomaProcessUpdateDto, SsomaProcess>();
            CreateMap<SsomaProcessDeleteDto, SsomaProcess>();
        }
    }
}
