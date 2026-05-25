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
    public class UpdateSupport(
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

        public async Task<BaseResponse> ExecuteAsync(SupportUpdateDto dto, long userId, long businessId)
        {
            var existing = await _repository.GetByIdAsync(dto.SupportId, businessId);
            if (existing == null) return null;

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<Core.Entities.Operations.Support>(dto);
                entity.UpdateUser = userId;
                entity.BusinessId = businessId;

                var result = await _repository.UpdateAsync(entity, transaction);

                var auditLog = new AuditLog
                {
                    BusinessId = businessId,
                    TableName = "SUPPORT",
                    RecordId = entity.SupportId,
                    CreateUser = userId
                };

                await _auditService.RegisterUpdateAsync(existing, entity, auditLog, trans: transaction);
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
