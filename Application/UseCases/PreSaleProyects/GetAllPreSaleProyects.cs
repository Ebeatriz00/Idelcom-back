using Application.DTOs.PreSaleProyects;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PreSaleProyects
{
    public class GetAllPreSaleProyects
    {
        private readonly IPreSaleProyectsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPreSaleProyects(IPreSaleProyectsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PreSaleProyectsResponseDto>> ExecuteAsync(
            long businessId,
            string? search,
            int page,
            int pageSize,
            long? workerId,
            string? filterCode = null,
            string? filterProject = null,
            string? filterClient = null,
            string? filterSeller = null,
            string? filterResponsible = null,
            string? filterStatePreSale = null,
            string? filterStateOpportunity = null,
            string? filterFinishDate = null,
            DateTime? filterDateFrom = null,
            DateTime? filterDateTo = null,
            string? opporNum = null,
            long? usersId = null, 
            string? sortBy = null,
            string? sortDirection = null,
            long? stateId = null,
            int? category = null,
            string? quoDate = null
        )
        {
            var entities = await _repository.GetAllAsync(
                businessId,
                search,
                page,
                pageSize,
                workerId,
                filterCode,
                filterProject,
                filterClient,
                filterSeller,
                filterResponsible,
                filterStatePreSale,
                filterStateOpportunity,
                filterFinishDate,
                filterDateFrom,
                filterDateTo,
                opporNum,
                usersId,
                sortBy,
                sortDirection,
                stateId,
                category,
                quoDate
            );

            return _mapper.Map<PagedResult<PreSaleProyectsResponseDto>>(entities);
        }
    }
}
