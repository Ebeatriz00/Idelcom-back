using Core.Interfaces.Ssoma;
using Core.Projections.Ssoma;

namespace Application.UseCases.SsomaHomologationPersonnel
{
    public class GetDetailPersonnelOperations(
        ISsomaHomologationPersonnelRepository repository)
    {
        private readonly ISsomaHomologationPersonnelRepository _repository = repository;

        public async Task<SsomaPersonnelOperationsItem?> ExecuteAsync(
            long personnelOperationsId,
            long businessId)
        {
            return await _repository.GetDetailPersonnelOperations(personnelOperationsId, businessId);
        }
    }
}
