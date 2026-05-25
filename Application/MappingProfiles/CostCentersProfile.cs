using Application.DTOs.CostCenters;
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
    public class CostCentersProfile : Profile
    {
        public CostCentersProfile()
        {
            CreateMap<CostCentersCreateDto, CostCenters>();
            CreateMap<CostCentersUpdateDto, CostCenters>();
            CreateMap<CostCenters, CostCentersResponseDto>();
            CreateMap<CostCenters, CostCentersSelectDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
