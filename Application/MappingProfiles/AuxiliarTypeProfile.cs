using Application.DTOs.AuxiliarType;
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
    public class AuxiliarTypeProfile : Profile
    {
        public AuxiliarTypeProfile()
        {
            CreateMap<AuxiliarType, AuxiliarTypeSelectDto>();
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
