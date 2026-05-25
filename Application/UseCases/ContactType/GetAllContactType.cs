using Application.DTOs.ContactType;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ContactType
{
    public class GetAllContactType
    {
        private readonly IContactTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllContactType(IContactTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ContactTypeResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<ContactTypeResponseDto>>(entities);
        }
    }
}
