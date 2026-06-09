using Application.DTOs.Ssoma.Clinics;
using Core.Entities.Ssoma.Clinics;
using Core.Interfaces.Ssoma.Clinics;
using Infrastructure.Persistence;
using SharedKernel;

namespace Application.UseCases.Ssoma.Clinics
{
    public class UpdateClinic(
        IClinicRepository repository,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        private readonly IClinicRepository _repository = repository;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(ClinicUpdateDto dto)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = new Clinic
                {
                    ClinicId = dto.ClinicId,
                    BusinessId = dto.BusinessId,
                    ClinicName = dto.ClinicName,
                    DocumentNumber = dto.DocumentNumber,
                    UpdateUser = dto.UpdateUser.ToString()
                };

                var updated = await _repository.UpdateAsync(entity, transaction);
                
                if (updated.Status == 0)
                {
                    throw new Exception(updated.Message ?? "Ocurrió un error al actualizar la clínica.");
                }

                transaction.Commit();
                return updated;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
