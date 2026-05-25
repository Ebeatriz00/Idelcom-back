using Application.DTOs.MovementTypes;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MovementTypes
{
    public class GetMovementTypesById
    {
        private readonly IMovementTypesRepository _repository;
        private readonly IMapper _mapper;

        public GetMovementTypesById(IMovementTypesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MovementTypesByIdDto> ExecuteAsync(long movementTypesId)
        {
            var entity = await _repository.GetByIdAsync(movementTypesId);
            return _mapper.Map<MovementTypesByIdDto>(entity);
        }
    }
}
