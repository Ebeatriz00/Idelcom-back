using Application.DTOs.CostCenters;
using Application.DTOs.Exercises;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Exercises
{
    public class GetAllExercises
    {
        private readonly IExercisesRepository _repository;
        private readonly IMapper _mapper;

        public GetAllExercises(IExercisesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ExercisesResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);

            return _mapper.Map<PagedResult<ExercisesResponseDto>>(entities);
        }
    }
}
