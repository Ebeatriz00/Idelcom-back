using Application.DTOs.Exercises;
using Application.UseCases.Modules;
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
    public class ExercisesProfile : Profile
    {
        public ExercisesProfile()
        {
            CreateMap<ExercisesCreateDto, Exercises>();
            CreateMap<ExercisesUpdateDto, Exercises>();
            CreateMap<Exercises, ExercisesResponseDto>();
            CreateMap<Exercises, ExercisesSelectDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
