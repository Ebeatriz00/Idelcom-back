using Application.DTOs.ProcessType;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProcessType
{
    public class GetByIdProcessType 
    {
        private readonly IProcessTypeReporsitory _repository;
        private readonly IMapper _mapper;

        public GetByIdProcessType(IProcessTypeReporsitory repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ProcessTypeResponseDto> ExecuteAsync(long processTypeId)
        {
            var entities = await _repository.GetByIdAsync(processTypeId);
            return _mapper.Map<ProcessTypeResponseDto>(entities);
        }
    }
}
