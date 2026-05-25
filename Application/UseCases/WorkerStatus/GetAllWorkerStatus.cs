using Application.DTOs.WorkerStatus;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.WorkerStatus
{
    public class GetAllWorkerStatus
    {
        private readonly IWorkerStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetAllWorkerStatus(IWorkerStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<WorkerStatusResponseDto>> ExecuteAsync(
            long businessId,
            string? search,
            int page,
            int pageSize,
            long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);

            return _mapper.Map<PagedResult<WorkerStatusResponseDto>>(entities);
        }
    }
}
