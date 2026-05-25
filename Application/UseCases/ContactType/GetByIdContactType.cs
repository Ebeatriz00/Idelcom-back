using Application.DTOs.ContactType;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ContactType
{
    public class GetContactTypeById
    {
        private readonly IContactTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetContactTypeById(IContactTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContactTypeResponseDto> ExecuteAsync(long contactTypeId)
        {
            var entity = await _repository.GetByIdAsync(contactTypeId);
            return _mapper.Map<ContactTypeResponseDto>(entity);
        }
    }
}
