using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Clients
{
    public class ExistsContacts
    {
        private readonly IClientsRepository _repository;
        public ExistsContacts(IClientsRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> ExecuteAsync(long clientsId)
        {
            return await _repository.ExistsContacts(clientsId);
        }
    }
}
