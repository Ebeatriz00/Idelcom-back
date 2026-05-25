using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;

namespace Application.UseCases.Operations.AssignmentStatus
{
    public class GetSelectAssignmentStatus(IAssignmentStatusRepository repository, IMapper mapper)
    {
        private readonly IAssignmentStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<AssignmentStatusSelectItem?>> ExecuteAsync(
            long businessId,
            int page,
            int pageSize,
            string? search)
        {
            var result = await _repository.GetForSelectAsync(businessId, page, pageSize, search);
            return _mapper.Map<PagedSelect<AssignmentStatusSelectItem?>>(result);
        }
    }

}
