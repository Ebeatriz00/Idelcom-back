using Application.DTOs.ContactsCrm;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ConctactsCrm
{
    public class GetContactsCrmById
    {
        private readonly IContactsCrmRepository _repository;
        private readonly IMapper _mapper;

        public GetContactsCrmById(IContactsCrmRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContactsCrmByIdDto> ExecuteAsync(long contactsCrmId)
        {
            var entity = await _repository.GetByIdAsync(contactsCrmId);
            return _mapper.Map<ContactsCrmByIdDto>(entity);
        }
    }
}
