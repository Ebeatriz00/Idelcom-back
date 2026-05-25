using Application.DTOs.Sector;
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
    public class SectorProfile : Profile
    {
        public SectorProfile()
        {
            CreateMap<SectorCreateDto, Sector>();
            CreateMap<SectorUpdateDto, Sector>();
            CreateMap<Sector, SectorResponseDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
