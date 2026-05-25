using Application.DTOs.SupplierGroups;
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
    public class SupplierGroupsProfile : Profile
    {
        public SupplierGroupsProfile()
        {
            CreateMap<SupplierGroupsCreateDto, SupplierGroups>();
            CreateMap<SupplierGroupsUpdateDto, SupplierGroups>();
            CreateMap<SupplierGroups, SupplierGroupsResponseDto>();
            CreateMap<SupplierGroups, SupplierGroupsSelectDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
