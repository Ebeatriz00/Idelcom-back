using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Operations.WorkDayStatus
{
    public class GetSelectWorkDayStatus(IWorkDayStatusRepository repository, IMapper mapper)
    {
        private readonly IWorkDayStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        public async Task<PagedSelect<WorkDayStatusSelectItem?>> ExecuteAsync(
            long businessId,
            int page,
            int pageSize,
            string? search)
        {
            var result = await _repository.GetForSelectAsync(businessId, page, pageSize, search);
            return _mapper.Map<PagedSelect<WorkDayStatusSelectItem?>>(result);
        }
    }
}
