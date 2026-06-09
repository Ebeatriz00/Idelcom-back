using Core.Interfaces.Ssoma.Clinics;
using Infrastructure.Persistence;
using SharedKernel;

namespace Application.UseCases.Ssoma.Clinics
{
    public class DeleteClinic(
        IClinicRepository repository,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        private readonly IClinicRepository _repository = repository;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(long clinicId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var deleted = await _repository.DeleteAsync(clinicId, userId, transaction);
                
                if (deleted.Status == 0)
                {
                    throw new Exception(deleted.Message ?? "Ocurrió un error al eliminar la clínica.");
                }

                transaction.Commit();
                return deleted;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
