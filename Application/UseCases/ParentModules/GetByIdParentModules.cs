using Application.DTOs.ParentModules;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ParentModules
{
    public class GetByIdParentModules
    {
        private readonly IParentModulesRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdParentModules(IParentModulesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ParentModulesResponseDto> ExecuteAsync(long parentModulesId)
        {
            var entities = await _repository.GetByIdAsync(parentModulesId);
            return _mapper.Map<ParentModulesResponseDto>(entities);
        }
    }
}
