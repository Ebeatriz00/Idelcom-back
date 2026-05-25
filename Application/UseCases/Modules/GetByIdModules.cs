using Application.DTOs.Modules;
using Application.DTOs.Profiles;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Modules
{
    public class GetByIdModules
    {
        private readonly IModulesRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdModules(IModulesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ModulesResponseDto> ExecuteAsync(long profilesId)
        {
            var entities = await _repository.GetByIdAsync(profilesId);
            return _mapper.Map<ModulesResponseDto>(entities);
        }
    }
}
