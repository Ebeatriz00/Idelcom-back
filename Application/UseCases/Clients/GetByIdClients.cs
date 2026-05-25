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
    public class GetByIdClients
    {
        private readonly IClientsRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdClients(IClientsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ClientsByIdDto> ExecuteAsync(long ClientsId)
        {
            var entity = await _repository.GetByIdAsync(ClientsId);
            return _mapper.Map<ClientsByIdDto>(entity);
        }
    }
}
