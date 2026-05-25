using Application.DTOs.OperationsProjectConfing;
using AutoMapper;
using Core.Entities.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles.Operations
{
    public class OperationsProjectConfigProfile : Profile
    {
        public OperationsProjectConfigProfile() { 
            CreateMap<OperationsProjectConfigCreateDto, OperationsProjectConfig>();
            CreateMap<OperationsProjectConfig, OperationsProjectConfigGetByIdDto>();
            CreateMap<OperationsProjectConfigUpdateDto, OperationsProjectConfig>();
        }
    }
}
