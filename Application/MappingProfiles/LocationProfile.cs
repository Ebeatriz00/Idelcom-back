using Application.DTOs.Location;
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
    public class LocationProfile : Profile
    {
        public LocationProfile() {
            CreateMap<Location, DepartmentDto>();
            CreateMap<Location, ProvinceDto>();
            CreateMap<Location, DistrictDto>();
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
