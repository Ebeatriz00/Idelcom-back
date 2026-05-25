using Application.DTOs.PreSaleProyects;
using Application.DTOs.ProjectTeam;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProjectTeam
{
    public class GetAllProjectTeam
    {
        private readonly IProjectTeamRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkToken;

        public GetAllProjectTeam(
            IProjectTeamRepository repository,
            IMapper mapper,
            ILinkTokenService linkToken) 
        {
            _repository = repository;
            _mapper = mapper;
            _linkToken = linkToken;
        }

        public async Task<PagedResult<ProjectTeamResponseDto>> ExecuteAsync(long businessId, string? projectToken, string? search, int page, int pageSize)
        {
            long? projectId = null;

            if (!string.IsNullOrWhiteSpace(projectToken))
            {
                if (_linkToken.TryValidate(projectToken, out var entity, out var resourceId) && entity == "opportunity")
                {
                    projectId = Convert.ToInt64(resourceId);
                }
            }

            var entities = await _repository.GetAllAsync(businessId, projectId, search, page, pageSize);
            return _mapper.Map<PagedResult<ProjectTeamResponseDto>>(entities);
        }
    }
}
