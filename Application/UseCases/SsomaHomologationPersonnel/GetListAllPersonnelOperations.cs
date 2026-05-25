using Core.Entities.paginations;
using Core.Interfaces.Ssoma;
using Core.Projections.Ssoma;

namespace Application.UseCases.SsomaHomologationPersonnel
{
    public class GetListAllPersonnelOperations(
        ISsomaHomologationPersonnelRepository repository)
    {
        private readonly ISsomaHomologationPersonnelRepository _repository = repository;

        public async Task<PagedResult<SsomaPersonnelOperationsListItem>> ExecuteAsync(
            long businessId,
            int page,
            int pageSize,
            string? search)
        {
          
            return await _repository.GetListAllPersonnelOperations(businessId, page, pageSize, search);
        }
    }
}
