using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System.Threading.Tasks;

namespace Application.UseCases.Worker
{
    public class GetSelectSquadWorker
    {
        private readonly IWorkerRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectSquadWorker(IWorkerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, long operationsId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetWorkerSquadSelectAsync(businessId, operationsId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
