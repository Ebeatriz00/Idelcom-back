using Application.DTOs.Clients;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Clients
{
    public class GetAllClients
    {
        private readonly IClientsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllClients(IClientsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ClientsResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy, bool? includeOthers)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy, includeOthers);
            return _mapper.Map<PagedResult<ClientsResponseDto>>(entities);
        }
    }
}
