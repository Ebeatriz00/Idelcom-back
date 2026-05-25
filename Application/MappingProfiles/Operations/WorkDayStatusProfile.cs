using Application.DTOs.Operations.WorkDayStatus;
using AutoMapper;
using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles.Operations
{
    public class WorkDayStatusProfile : Profile  
    {
        public WorkDayStatusProfile()
        {
            CreateMap<WorkDayStatusSelectItem, WorkDayStatusSelectDto>();
            CreateMap<WorkDayStatus, WorkDayStatusGetByIdDto>();


            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
