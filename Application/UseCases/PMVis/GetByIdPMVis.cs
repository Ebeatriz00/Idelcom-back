using Application.DTOs.PMVis;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PMVis
{
    public class GetPMVisById
    {
        private readonly IPMVisRepository _repository;
        private readonly IMapper _mapper;

        public GetPMVisById(IPMVisRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PMVisResponseDto> ExecuteAsync(long pmVisId)
        {
            var entity = await _repository.GetByIdAsync(pmVisId);
            return _mapper.Map<PMVisResponseDto>(entity);
        }
    }
}
