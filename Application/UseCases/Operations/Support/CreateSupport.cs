using Application.DTOs.Operations.Support;
using AutoMapper;
using Core.Entities.Audit;
using Core.Entities.Operations;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Infrastructure.Persistence;
using SharedKernel;

namespace Application.UseCases.Operations.Support
{
    public class CreateSupport(
        ISupportRepository repository,
        IAuditService auditService,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory
    )
    {
        private readonly ISupportRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponseId> ExecuteAsync(SupportCreateDto dto, long userId, long businessId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<Core.Entities.Operations.Support>(dto);
                entity.CreateUser = userId;
                entity.BusinessId = businessId;

                var created = await _repository.CreateAsync(entity, transaction);

                if (created.Status == 0 || created.Id == null || created.Id <= 0)
                {
                    throw new Exception(created.Message ?? "Ocurrió un error al crear el apoyo en la BD.");
                }

                entity.SupportId = (long)created.Id;

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = "SUPPORT",
                    RecordId = entity.SupportId,
                    CreateUser = userId
                };

                await _auditService.RegisterCreateAsync(entity, auditLog, trans: transaction);
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
