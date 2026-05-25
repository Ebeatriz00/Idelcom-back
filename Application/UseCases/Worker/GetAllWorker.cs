using Application.DTOs.Worker;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Worker
{
    public class GetAllWorker
    {
        private readonly IWorkerRepository _repository;
        private readonly IMapper _mapper;

        public GetAllWorker(IWorkerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<WorkerResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId,search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<WorkerResponseDto>>(entities);
        }
    }
}
