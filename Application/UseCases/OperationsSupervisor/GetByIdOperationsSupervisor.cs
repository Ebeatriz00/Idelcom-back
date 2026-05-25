using Application.DTOs.OperationsSupervisor;
using AutoMapper;
using Core.Interfaces.OperationsSupervisor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsSupervisor
{
    public class GetByIdOperationsSupervisor(IOperationsSupervisorRepository repository, IMapper mapper)
    {
        private readonly IOperationsSupervisorRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsSupervisorResponseDto?> ExecuteAsync(long supervisorId)
        {
            var entity = await _repository.GetByIdAsync(supervisorId);
            return _mapper.Map<OperationsSupervisorResponseDto?>(entity);
        }
    }
}