using Application.DTOs.Viability;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Viability
{
    public class GetAllViability
    {
        private readonly IViabilityRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkTokenService;
        public GetAllViability(IViabilityRepository repository, IMapper mapper, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _mapper = mapper;
            _linkTokenService = linkTokenService;
        }
        public async Task<PagedResult<ViabilityResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var pagedEntities = await _repository.GetAllAsync(businessId, search, page, pageSize);

            if (pagedEntities?.Items == null) return _mapper.Map<PagedResult<ViabilityResponseDto>>(pagedEntities);

            foreach (var entity in pagedEntities.Items)
            {
                if (long.TryParse(entity.OpporToken, out long opporId))
                {
                    entity.OpporToken = _linkTokenService.Issue("opportunity", opporId, TimeSpan.FromHours(24));
                }
            }
            return _mapper.Map<PagedResult<ViabilityResponseDto>>(pagedEntities);
        }
    }
}
