using Application.DTOs.Worker;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Worker
{
        public class GetByIdWorker
        {
            private readonly IWorkerRepository _repository;
            private readonly IMapper _mapper;

            public GetByIdWorker(IWorkerRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<WorkerResponseByIdDto> ExecuteAsync(long workerId)
            {
                var entity = await _repository.GetByIdAsync(workerId);
                return _mapper.Map<WorkerResponseByIdDto>(entity);
            }
        }

}
