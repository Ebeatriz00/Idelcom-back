using Application.DTOs.Operations.MovementStatus;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.MovementStatus
{
    public class GetSelectMovementStatus(IMovementStatusRepository repository, IMapper mapper)
    {
        private readonly IMovementStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<MovementStatusGetSelectDto>> ExecuteAsync(long businessId, int page, int pageSize, string? search)
        {
            var result = await _repository.GetSelectAsync(businessId, page, pageSize, search);

            return _mapper.Map<PagedSelect<MovementStatusGetSelectDto>>(result);
        }
    }
}
