using Application.DTOs.OperationsTeamSsoma;
using Application.Contexts.OperationsTeamSsoma;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.OperationsTeamSsoma
{
    public class UpdateOperationsTeamSsoma(
        IOperationsTeamSsomaRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsTeamSsomaUpdateDto> validator)
    {
        private readonly IOperationsTeamSsomaRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsTeamSsomaUpdateDto> _validator = validator;

        public async Task<GlobalResponse> ExecuteAsync(
            OperationsTeamSsomaUpdateDto dto,
            long userId,
            long businessId)
        {
            dto.BusinessId = businessId;
            dto.UpdateUser = userId;

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
                var before = await _repository.GetByProcessIdAsync(businessId, dto.SsomaProcessId);

                if (before is null)
                    throw new InvalidOperationException("No se encontró el equipo SSOMA.");

                var entity = _mapper.Map<Core.Entities.Operations.OperationsTeamSsoma>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var beforeItems = before.TeamSsoma ?? new();
                var fullRequestItems = _mapper.Map<List<OperationsTeamSsomaAssignmentItem>>(dto.TeamSsoma);
                var itemsToPersist = GetItemsToPersist(beforeItems, fullRequestItems);

                if (itemsToPersist.Count == 0)
                {
                    return new GlobalResponse
                    {
                        Status = 1,
                        Message = "No se detectaron cambios en el equipo SSOMA.",
                        Id = dto.SsomaProcessId
                    };
                }

                var result = await _repository.UpdateAsync(entity, itemsToPersist, transaction);

                if (result.Status <= 0)
                    throw new InvalidOperationException("No se pudo actualizar.");

                var after = await _repository.GetByProcessIdAsync(
                    businessId,
                    dto.SsomaProcessId,
                    transaction);

                if (after is null)
                    throw new InvalidOperationException("No se pudo recuperar el equipo actualizado.");

                var beforeHeader = new OperationsTeamSsomaAuditSnapshot
                {
                    BusinessId = before.BusinessId,
                    SsomaProcessId = before.SsomaProcessId,
                    AssignmentId = before.AssignmentId,
                    IsActive = before.IsActive,
                    ReasonChange = before.ReasonChange,
                    ReplacedAssignmentId = before.ReplacedAssignmentId,
                    ClientApprovalStatusId = before.ClientApprovalStatusId,
                    ClientApprovalDate = before.ClientApprovalDate,
                    Comments = before.Comments
                };

                var afterHeader = new OperationsTeamSsomaAuditSnapshot
                {
                    BusinessId = after.BusinessId,
                    SsomaProcessId = after.SsomaProcessId,
                    AssignmentId = after.AssignmentId,
                    IsActive = after.IsActive,
                    ReasonChange = after.ReasonChange,
                    ReplacedAssignmentId = after.ReplacedAssignmentId,
                    ClientApprovalStatusId = after.ClientApprovalStatusId,
                    ClientApprovalDate = after.ClientApprovalDate,
                    Comments = after.Comments
                };

                var headerAuditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.OperationsTeamSsoma,
                    dto.SsomaProcessId,
                    userId);

                await _auditService.RegisterUpdateAsync(
                    beforeHeader,
                    afterHeader,
                    headerAuditLog,
                    transaction);

                var afterItems = after.TeamSsoma ?? new();

                static string BuildKey(OperationsTeamSsomaAssignmentItem item) =>
                    item.OperationsTeamSsomaId.HasValue && item.OperationsTeamSsomaId > 0
                        ? $"ID:{item.OperationsTeamSsomaId}"
                        : $"W:{item.WorkerId}-R:{item.SsomaRoleId}-S:{item.StartDate:yyyyMMdd}";

                var beforeMap = beforeItems
                    .GroupBy(BuildKey)
                    .ToDictionary(g => g.Key, g => g.First());

                var afterMap = afterItems
                    .GroupBy(BuildKey)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var itemAfter in afterItems)
                {
                    var key = BuildKey(itemAfter);

                    if (beforeMap.TryGetValue(key, out var itemBefore))
                    {
                        if (AreEquivalent(itemBefore, itemAfter))
                            continue;

                        var auditLog = _auditLogFactory.Create(
                            businessId,
                            TableNames.OperationsTeamSsoma,
                            itemAfter.OperationsTeamSsomaId ?? dto.SsomaProcessId,
                            userId);

                        await _auditService.RegisterUpdateAsync(itemBefore, itemAfter, auditLog, transaction);
                    }
                    else
                    {
                        var auditLog = _auditLogFactory.Create(
                            businessId,
                            TableNames.OperationsTeamSsoma,
                            itemAfter.OperationsTeamSsomaId ?? dto.SsomaProcessId,
                            userId);

                        await _auditService.RegisterCreateAsync(itemAfter, auditLog, transaction);
                    }
                }

                foreach (var itemBefore in beforeItems)
                {
                    var key = BuildKey(itemBefore);

                    if (!afterMap.ContainsKey(key))
                    {
                        var auditLog = _auditLogFactory.Create(
                            businessId,
                            TableNames.OperationsTeamSsoma,
                            itemBefore.OperationsTeamSsomaId ?? dto.SsomaProcessId,
                            userId);

                        await _auditService.RegisterDeleteAsync(itemBefore, auditLog, transaction);
                    }
                }

                transaction.Commit();
                return result;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al actualizar SSOMA.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static List<OperationsTeamSsomaAssignmentItem> GetItemsToPersist(
            List<OperationsTeamSsomaAssignmentItem> beforeItems,
            List<OperationsTeamSsomaAssignmentItem> requestItems)
        {
            var beforeById = beforeItems
                .Where(item => item.OperationsTeamSsomaId.HasValue && item.OperationsTeamSsomaId.Value > 0)
                .ToDictionary(item => item.OperationsTeamSsomaId!.Value);

            var requestIds = requestItems
                .Where(item => item.OperationsTeamSsomaId.HasValue && item.OperationsTeamSsomaId.Value > 0)
                .Select(item => item.OperationsTeamSsomaId!.Value)
                .ToHashSet();

            var hasPotentialInsert = requestItems.Any(item =>
                !item.OperationsTeamSsomaId.HasValue ||
                item.OperationsTeamSsomaId.Value <= 0 ||
                !beforeById.ContainsKey(item.OperationsTeamSsomaId.Value));

            var hasPotentialDelete = beforeById.Keys.Any(existingId => !requestIds.Contains(existingId));

            if (hasPotentialInsert || hasPotentialDelete)
                return requestItems;

            return requestItems
                .Where(item =>
                    item.OperationsTeamSsomaId.HasValue &&
                    beforeById.TryGetValue(item.OperationsTeamSsomaId.Value, out var beforeItem) &&
                    !AreEquivalent(beforeItem, item))
                .ToList();
        }

        private static bool AreEquivalent(OperationsTeamSsomaAssignmentItem beforeItem, OperationsTeamSsomaAssignmentItem afterItem) =>
            beforeItem.OperationsTeamSsomaId == afterItem.OperationsTeamSsomaId &&
            beforeItem.WorkerId == afterItem.WorkerId &&
            beforeItem.SsomaRoleId == afterItem.SsomaRoleId &&
            beforeItem.StartDate == afterItem.StartDate &&
            beforeItem.EndDate == afterItem.EndDate &&
            beforeItem.IsPrimary == afterItem.IsPrimary;
    }
}
