using Application.DTOs.Boxes;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Boxes
{
    public class GetBoxesById
    {
        private readonly IBoxesRepository _repository;
        private readonly IMapper _mapper;

        public GetBoxesById(IBoxesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BoxesByIdDto> ExecuteAsync(long boxesId)
        {
            var entity = await _repository.GetByIdAsync(boxesId);
            return _mapper.Map<BoxesByIdDto>(entity);
        }
    }
}
