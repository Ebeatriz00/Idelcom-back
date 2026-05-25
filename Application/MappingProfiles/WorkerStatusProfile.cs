using Application.DTOs.WorkerStatus;
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
    public class WorkerStatusProfile : Profile
    {
        public WorkerStatusProfile()
        {
            CreateMap<WorkerStatusCreateDto, WorkerStatus>();
            CreateMap<WorkerStatusUpdateDto, WorkerStatus>();
            CreateMap<WorkerStatus, WorkerStatusResponseDto>();
            CreateMap<WorkerStatus, WorkerStatusSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
