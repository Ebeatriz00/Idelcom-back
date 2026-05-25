using Application.DTOs.WorkerStatus;
using AutoMapper;
using Core.Interfaces;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.WorkerStatus
{
    public class CreateWorkerStatus
    {
        private readonly IWorkerStatusRepository _repository;
        private readonly IMapper _mapper;

        public CreateWorkerStatus(IWorkerStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(WorkerStatusCreateDto dto)
        {
            var entity = _mapper.Map<Core.Entities.WorkerStatus>(dto);

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Estado del trabajador creado exitosamente."
            };
        }
    }
}
