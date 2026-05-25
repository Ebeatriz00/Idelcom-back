using Application.DTOs.OperationsTeamSsoma;
using AutoMapper;
using Core.Interfaces.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsTeamSsoma
{
    public class GetByIdOperationsTeamSsoma(
        IOperationsTeamSsomaRepository repository,
        IMapper mapper)
    {
        private readonly IOperationsTeamSsomaRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsTeamSsomaGetByIdDto?> ExecuteAsync(long operationsTeamSsomaId)
        {
            var entity = await _repository.GetByIdAsync(operationsTeamSsomaId);
            return _mapper.Map<OperationsTeamSsomaGetByIdDto?>(entity);
        }
    }
}
