using Application.DTOs.OperationsSquad;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.OperationsSquad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsSquad
{
    public class GetAllOperationsSquad(IOperationsSquadRepository repository, IMapper mapper)
    {
        private readonly IOperationsSquadRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<OperationsSquadResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? workOrderId)
        {
            var pagedEntity = await _repository.GetAllAsync(businessId, search, page, pageSize, workOrderId);
            return _mapper.Map<PagedResult<OperationsSquadResponseDto>>(pagedEntity);
        }
    }
}
