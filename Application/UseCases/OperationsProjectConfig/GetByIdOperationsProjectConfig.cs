using Application.DTOs.OperationsProjectConfing;
using AutoMapper;
using Core.Interfaces.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsProjectConfig
{
    public class GetByIdOperationsProjectConfig(
        IOperationsProjectConfigRepository repository,
        IMapper mapper
    )
    {
        private readonly IOperationsProjectConfigRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsProjectConfigGetByIdDto?> ExecuteAsync(long operationsProjectConfigId, long operationsId)
        {
            var entity = await _repository.GetByIdAsync(operationsProjectConfigId, operationsId);
            return _mapper.Map<OperationsProjectConfigGetByIdDto?>(entity);
        }
    }
}
