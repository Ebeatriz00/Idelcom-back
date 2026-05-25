using Application.DTOs.Worker;
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
        public class WorkerProfile : Profile
        {
            public WorkerProfile()
            {
                CreateMap<WorkerCreateDto, Worker>();
                CreateMap<WorkerUpdateDto, Worker>();
                CreateMap<Worker, WorkerResponseByIdDto>();
                CreateMap<Worker, WorkerResponseDto>();
                CreateMap<Worker, WorkerSelectDto>();

                CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
                CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
        }
}
