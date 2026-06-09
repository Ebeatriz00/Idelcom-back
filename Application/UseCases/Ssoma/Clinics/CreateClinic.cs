using Application.DTOs.Ssoma.Clinics;
using AutoMapper;
using Core.Entities.Ssoma.Clinics;
using Core.Interfaces.Ssoma.Clinics;
using Infrastructure.Persistence;
using SharedKernel;

namespace Application.UseCases.Ssoma.Clinics
{
    public class CreateClinic(
        IClinicRepository repository,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        private readonly IClinicRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponseId> ExecuteAsync(ClinicCreateDto dto)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = new Clinic
                {
                    BusinessId = dto.BusinessId,
                    ClinicName = dto.ClinicName,
                    DocumentNumber = dto.DocumentNumber,
                    CreateUser = dto.CreateUser
                };

                var created = await _repository.CreateAsync(entity, transaction);

                if (created.Status == 0 || created.Id == null || created.Id <= 0)
                {
                    throw new Exception(created.Message ?? "Ocurrió un error al crear la clínica en la BD.");
                }

                transaction.Commit();
                return created;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
