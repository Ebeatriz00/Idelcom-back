using Application.DTOs.OperationsProjectConfing;
using AutoMapper;
using Core.Interfaces.Operations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsProjectConfig
{
    public class GetAllOperationsProjectConfig(
        IOperationsProjectConfigRepository repository,
        IMapper mapper
    )
    {
        private readonly IOperationsProjectConfigRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<OperationsProjectConfigGetByIdDto>> ExecuteAsync(long operationsId)
        {
            var entities = await _repository.GetAllAsync(operationsId);
            return _mapper.Map<IEnumerable<OperationsProjectConfigGetByIdDto>>(entities);
        }
    }
}
