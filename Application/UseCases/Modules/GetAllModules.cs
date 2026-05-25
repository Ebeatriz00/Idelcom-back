using Application.DTOs.Modules;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Modules
{
    public class GetAllModules
    {
        private readonly IModulesRepository _repository;
        private readonly IMapper _mapper;
        public GetAllModules(IModulesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<ModulesResponseDto>> ExecuteAsync(long businessId, long parentModulesId, string? search, long? usersId)
        {
            var entities = await _repository.GetAllAsync(businessId, parentModulesId, search, usersId);
            return _mapper.Map<List<ModulesResponseDto>>(entities);
        }
    }
}
