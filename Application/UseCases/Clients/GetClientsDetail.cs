using Application.DTOs.Clients;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Clients
{
    public class GetClientsDetail
    {
        private readonly IClientsRepository _repository;

        public GetClientsDetail(IClientsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Core.Entities.ClientsDetailDto> ExecuteAsync(long clientsId, long businessId)
        {
            var result = await _repository.GetClientsDetailAsync(clientsId, businessId);

            if (result == null || result.Header.ClientsId == 0)
            {
                return null;
            }

            return result;
        }
    }
}
