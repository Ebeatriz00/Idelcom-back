using Application.DTOs.OperationsSupervisor;
using AutoMapper;
using Core.Entities.OperationsSupervisor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class OperationsSupervisorProfile : Profile
    {
        public OperationsSupervisorProfile() 
        {
            CreateMap<OperationsSupervisorCreateDto, OperationSupervisor>();
            CreateMap<OperationSupervisor, OperationsSupervisorResponseDto>();
            CreateMap<OperationsSupervisorUpdateDto, OperationSupervisor>();
        }
    }
}
