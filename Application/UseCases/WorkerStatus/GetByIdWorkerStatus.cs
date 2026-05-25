using Application.DTOs.SsomaMovementType;
using Application.DTOs.WorkerStatus;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.WorkerStatus
{
    public class GetByIdWorkerStatus
    {
        private readonly IWorkerStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdWorkerStatus(IWorkerStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<WorkerStatusResponseDto> ExecuteAsync(long workerStatusId)
        {
            var entity = await _repository.GetByIdAsync(workerStatusId);
            return _mapper.Map<WorkerStatusResponseDto>(entity);
        }
    }
}
