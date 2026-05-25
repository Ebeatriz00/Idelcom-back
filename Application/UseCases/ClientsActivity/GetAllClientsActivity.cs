using Application.DTOs.ClientsActivity;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ClientsActivity
{
    public class GetClientsActivities
    {
        private readonly IClientsActivityRepository _repository;
        private readonly IMapper _mapper;

        public GetClientsActivities(IClientsActivityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ClientsActivityResponseDto>> ExecuteAsync(long businessId, long clientsId, int page, int pageSize)
        {
            var pagedEntities = await _repository.GetActivitiesAllAsync(businessId, clientsId, page, pageSize);

            return _mapper.Map<PagedResult<ClientsActivityResponseDto>>(pagedEntities);
        }
    }
}
