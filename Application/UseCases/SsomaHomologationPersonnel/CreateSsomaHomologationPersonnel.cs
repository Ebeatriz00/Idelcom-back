using Application.DTOs.SsomaHomologationPersonnel;
using Application.UseCases.SsomaHomologationPersonnel.services;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.SsomaHomologationPersonnel
{
    public class CreateSsomaHomologationPersonnel(
        ISsomaHomologationPersonnelRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaHomologationPersonnelCreateDto> validator,
        SsomaHomologationPersonnelBusinessRules businessRules,
        ISsomaHomologationCalculator homologationCalculator,
        IOperationsRepository operationsRepository)
    {
        private readonly ISsomaHomologationPersonnelRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaHomologationPersonnelCreateDto> _validator = validator;
        private readonly SsomaHomologationPersonnelBusinessRules _businessRules = businessRules;
        private readonly ISsomaHomologationCalculator _homologationCalculator = homologationCalculator;
        private readonly IOperationsRepository _operationsRepository = operationsRepository;

        public async Task<BaseResponseId> ExecuteAsync(SsomaHomologationPersonnelCreateDto dto, long userId, long businessId)
        {
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
                DateTime? operationEndDate = null;

                if (dto.HomologationScopeId == 2)
                {
                    if (!dto.OperationsId.HasValue)
                    {
                        throw new AppValidationException(new List<GlobalErrorDetail>
                        {
                            new("OPERATIONS_REQUIRED", "La operacion es obligatoria para homologacion por operacion.")
                        });
                    }

                    operationEndDate = await _operationsRepository.GetOperationEndDateAsync(
                        dto.OperationsId.Value,
                        transaction);

                    if (!operationEndDate.HasValue)
                    {
                        throw new AppValidationException(new List<GlobalErrorDetail>
                        {
                            new("OPERATION_END_DATE_NOT_FOUND", "No se encontro la fecha fin de la operacion.")
                        });
                    }
                }

                var orchestratedDto = new SsomaHomologationPersonnelCreateOrchestratedDto
                {
                    HomologationPersonnel = dto
                };

                dto.ValidTo = _homologationCalculator.CalculateValidTo(orchestratedDto, operationEndDate);
                dto.WorkerStatusId = _homologationCalculator.CalculateWorkerStatusId(orchestratedDto, dto.ValidTo!.Value);

                _businessRules.Normalize(
                    dto.OperationsId,
                    dto.ValidFrom!.Value,
                    dto.ValidTo!.Value,
                    dto.Notes,
                    out var normalizedOperationsId,
                    out var normalizedValidFrom,
                    out var normalizedValidTo,
                    out var normalizedNotes);

                dto.OperationsId = normalizedOperationsId;
                dto.ValidFrom = normalizedValidFrom;
                dto.ValidTo = normalizedValidTo;
                dto.Notes = normalizedNotes;

                await _businessRules.ValidateReferencesAsync(
                    businessId,
                    dto.WorkerId,
                    dto.WorkerStatusId,
                    dto.MedicalAptitudeId,
                    dto.OperationsId);

                var entity = _mapper.Map<Core.Entities.Ssoma.SsomaHomologationPersonnel>(dto);
                entity.BusinessId = businessId;

                var existing = await _repository.GetActiveByBusinessWorkerScopeAsync(
                    businessId,
                    entity.WorkerId,
                    entity.HomologationScopeId,
                    entity.OperationsId,
                    transaction);

                if (existing != null)
                {
                    entity.HomologationPersonnelId = existing.HomologationPersonnelId;
                    entity.UpdateUser = userId;

                    var updated = await _repository.UpdateAsync(entity, transaction);

                    var after = await _repository.GetByIdAsync(existing.HomologationPersonnelId, businessId, transaction);
                    if (after == null)
                        throw new BusinessException("No se pudo recuperar la homologacion de personal SSOMA actualizada.");

                    var updateAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.SsomaHomologationPersonnel,
                        existing.HomologationPersonnelId,
                        userId);

                    await _auditService.RegisterUpdateAsync(existing, after, updateAuditLog, transaction);

                    transaction.Commit();
                    return new BaseResponseId
                    {
                        Status = updated.Status,
                        Message = updated.Message,
                        Id = existing.HomologationPersonnelId
                    };
                }

                entity.CreateUser = userId;

                var created = await _repository.CreateAsync(entity, transaction);

                if (created.Id == null || created.Id <= 0)
                    throw new Exception("No se pudo crear la homologación de personal SSOMA.");

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.SsomaHomologationPersonnel,
                    (long)created.Id,
                    userId);

                entity.HomologationPersonnelId = (long)created.Id;
                await _auditService.RegisterCreateAsync(entity, auditLog, trans: transaction);

                transaction.Commit();
                return created;
            }
            catch (BaseException)
            {
                transaction.Rollback();
                throw;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al registrar la homologación de personal SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al guardar la homologación de personal SSOMA.", ex.Message);
            }
        }
    }
}
