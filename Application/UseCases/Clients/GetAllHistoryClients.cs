using Application.DTOs.Clients;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Clients
{
    public class GetAllHistoryClients
    {
        private readonly IClientsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllHistoryClients(IClientsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<ClientsHistoryResponseDto>> ExecuteAsync(long? clientsId)
        {
            var entities = await _repository.GetHistoryAsync(clientsId);
            return _mapper.Map<List<ClientsHistoryResponseDto>>(entities);
        }
    }
}
