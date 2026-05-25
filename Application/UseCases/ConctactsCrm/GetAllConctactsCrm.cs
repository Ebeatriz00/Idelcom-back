using Application.DTOs.ContactsCrm;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ConctactsCrm
{
    public class GetAllContactsCrm
    {
        private readonly IContactsCrmRepository _repository;
        private readonly IMapper _mapper;

        public GetAllContactsCrm(IContactsCrmRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ContactsCrmResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {

            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<ContactsCrmResponseDto>>(entities);
        }
    }
}
