using Application.DTOs.StateTask;
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
    public class StateTaskProfile : Profile
    {
        public StateTaskProfile()
        {
            CreateMap<StateTaskCreateDto, StateTask>();
            CreateMap<StateTaskUpdateDto, StateTask>();
            CreateMap<StateTask, StateTaskResponseDto>();
            CreateMap<StateTask, StateTaskSelectDto>();
            CreateMap<StateTask, StateTaskByIdDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));

        }
    }
}
