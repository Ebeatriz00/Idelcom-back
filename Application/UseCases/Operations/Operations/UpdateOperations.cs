using Application.DTOs.Operations.Operations;
using AutoMapper;
using Core.Entities.Operations;
using Core.Interfaces;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Operations.Operations
{
    public class UpdateOperations(
        IOperationsRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsUpdateDto> validator,
        IStorageService storageService
        )
    {
        private readonly IOperationsRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsUpdateDto> _validator = validator;
        private readonly IStorageService _storageService = storageService;

        public async Task<BaseResponse> ExecuteAsync(OperationsUpdateDto dto, long userId, long businessId)
        {
            var before = await _repository.GetByIdAsync(dto.OperationsId);

            if (before == null)
                throw new Exception("No se encontró la operación.");

            if (dto.OpporId <= 0)
            {
                dto.OpporId = before.OpporId;
            }

            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<Operation>(dto);

                if (dto.ClosurePdfFile != null)
                {
                    using var stream = dto.ClosurePdfFile.OpenReadStream();
                    entity.ClosurePdfFileUid = await _storageService.UploadAsync(
                        stream,
                        dto.ClosurePdfFile.FileName,
                        "Operations/ClosureAct/PDF",
                        userId);
                }

                var updated = await _repository.UpdateAsync(entity, userId, businessId, transaction);

                entity.BusinessId = businessId;

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.Operations,
                    before.OperationsId,
                    userId);

                var beforeEntity = _mapper.Map<Operation>(before);

                await _auditService.RegisterUpdateAsync(beforeEntity, entity, auditLog, trans: transaction);

                transaction.Commit();
                return updated;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
