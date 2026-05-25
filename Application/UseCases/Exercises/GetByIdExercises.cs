using Application.DTOs.CostCenters;
using Application.DTOs.Exercises;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Exercises
{
    public class GetByIdExercises
    {
        private readonly IExercisesRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdExercises(IExercisesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ExercisesResponseDto> ExecuteAsync(long exercisesId)
        {
            var entity = await _repository.GetByIdAsync(exercisesId);

            return _mapper.Map<ExercisesResponseDto>(entity);
        }
    }
}
