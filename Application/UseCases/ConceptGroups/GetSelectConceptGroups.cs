using Application.DTOs.ConceptGroups;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.UseCases.ConceptGroups
{
    public class GetSelectConceptGroups
    {
        private readonly IConceptGroupsRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectConceptGroups(IConceptGroupsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetForSelectAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
