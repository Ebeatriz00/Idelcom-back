using Application.DTOs.TypeAnalysis;
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
    public class TypeAnalysisProfile : Profile
    {
        public TypeAnalysisProfile()
        {
             CreateMap<TypeAnalysis, TypeAnalysisSelectDto>();
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
