using Application.DTOs.OperationsTeamSsoma;
using AutoMapper;
using Core.Entities.OperationsTeamSsomaMovement;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Core.Interfaces.OperationsSsomaMovement;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.OperationsTeamSsoma
{
    public class ProcessSsomaAssignmentChange(
        IOperationsTeamSsomaRepository repository,
        IOperationsTeamSsomaMovementRepository repositoryMovement,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<ProcessSsomaAssignmentChangeDto> validator)
    {
        private readonly IOperationsTeamSsomaRepository _repository = repository;
        private readonly IOperationsTeamSsomaMovementRepository _repositoryMovement = repositoryMovement;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<ProcessSsomaAssignmentChangeDto> _validator = validator;

        public async Task<GlobalResponse> ExecuteAsync(ProcessSsomaAssignmentChangeDto dto, long userId, long businessId)
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
                var before = await _repository.GetByIdAsync(dto.OperationsTeamSsomaId, transaction);

                if (before is null)
                    throw new InvalidOperationException("No se encontró la asignación SSOMA a procesar.");

                var sourceSsomaProcessId = before.SsomaProcessId;

                if (dto.FromSsomaProcessId.HasValue && dto.FromSsomaProcessId.Value != sourceSsomaProcessId)
                    throw new InvalidOperationException("El proceso origen no coincide con la asignación SSOMA seleccionada.");

                var beforeItem = before.TeamSsoma?
                    .FirstOrDefault(x => x.OperationsTeamSsomaId == dto.OperationsTeamSsomaId);

                if (beforeItem is null)
                    throw new InvalidOperationException("No se pudo recuperar el detalle de la asignación SSOMA seleccionada.");

                ValidateBusinessRules(dto, before, beforeItem, sourceSsomaProcessId);

                var entity = _mapper.Map<Core.Entities.Operations.OperationsTeamSsoma>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;
                entity.CreateUser = userId;

                var shouldCloseCurrentAssignment =
                    dto.ChangeType == SsomaAssignmentChangeType.Update ||
                    before.IsActive;

                GlobalResponse changeResult;

                if (shouldCloseCurrentAssignment)
                {
                    var statusResult = await _repository.UpdateTeamSssomaAssignmentStatusAsync(entity, transaction);

                    if (statusResult.Status <= 0)
                        throw new InvalidOperationException("No se pudo actualizar el estado de la asignación SSOMA.");

                    changeResult = statusResult;
                }
                else
                {
                    changeResult = new GlobalResponse
                    {
                        Status = 1,
                        Id = dto.OperationsTeamSsomaId
                    };
                }

                var targetOperationsTeamSsomaId = dto.OperationsTeamSsomaId;

                switch (dto.ChangeType)
                {
                    case SsomaAssignmentChangeType.Relocation:
                        changeResult = await _repository.RelocationTeamSsomaAsync(entity, transaction);
                        break;

                    case SsomaAssignmentChangeType.Replacement:
                        changeResult = await _repository.ReplacementTeamSsomaAsync(entity, transaction);
                        break;

                    case SsomaAssignmentChangeType.Update:
                        break;

                    default:
                        throw new InvalidOperationException("Tipo de cambio SSOMA no válido.");
                }

                if (changeResult.Status <= 0)
                    throw new InvalidOperationException("No se pudo procesar el cambio de asignación SSOMA.");

                if (changeResult.Id is > 0)
                    targetOperationsTeamSsomaId = changeResult.Id.Value;

                if (dto.ChangeType == SsomaAssignmentChangeType.Replacement &&
                    !dto.ReplacedAssignmentId.HasValue &&
                    !before.ReplacedAssignmentId.HasValue &&
                    targetOperationsTeamSsomaId != dto.OperationsTeamSsomaId)
                {
                    var replacedAssignmentResult = await _repository.UpdateReplacedAssignmentIdAsync(
                        businessId,
                        dto.OperationsTeamSsomaId,
                        targetOperationsTeamSsomaId,
                        userId,
                        transaction);

                    if (replacedAssignmentResult.Status <= 0)
                        throw new InvalidOperationException("No se pudo actualizar la asignación reemplazada del registro SSOMA origen.");
                }

                var after = await _repository.GetByIdAsync(targetOperationsTeamSsomaId, transaction);

                if (after is null)
                    throw new InvalidOperationException("No se pudo recuperar la asignación SSOMA procesada.");

                if (shouldCloseCurrentAssignment && beforeItem is not null)
                {
                    var updatedItem = BuildUpdatedItem(beforeItem, dto);
                    var statusAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.OperationsTeamSsoma,
                        dto.OperationsTeamSsomaId,
                        userId);

                    await _auditService.RegisterUpdateAsync(beforeItem, updatedItem, statusAuditLog, transaction);
                }

                if (dto.ChangeType is SsomaAssignmentChangeType.Relocation or SsomaAssignmentChangeType.Replacement)
                {
                    var afterItem = after.TeamSsoma?
                        .FirstOrDefault(x => x.OperationsTeamSsomaId == targetOperationsTeamSsomaId);

                    if (afterItem is null)
                        throw new InvalidOperationException("No se pudo recuperar el nuevo registro SSOMA generado.");

                    var assignmentAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.OperationsTeamSsoma,
                        afterItem.OperationsTeamSsomaId ?? targetOperationsTeamSsomaId,
                        userId);

                    await _auditService.RegisterCreateAsync(afterItem, assignmentAuditLog, transaction);
                }

                var movement = new OperationsTeamSsomaMovement
                {
                    BusinessId = businessId,
                    OperationsTeamSsomaId = targetOperationsTeamSsomaId,
                    SssomaMovementTypeId = dto.SssomaMovementTypeId,
                    MovementDate = dto.MovementDate,
                    FromSsomaProcessId = sourceSsomaProcessId,
                    ToSsomaProcessId = ResolveTargetSsomaProcessId(dto, sourceSsomaProcessId),
                    Description = string.IsNullOrWhiteSpace(dto.Description) ? dto.ReasonChange : dto.Description,
                    CreateUser = userId
                };

                await _repositoryMovement.AddAsync(movement, transaction);

                transaction.Commit();

                return new GlobalResponse
                {
                    Status = changeResult.Status,
                    Message = string.IsNullOrWhiteSpace(changeResult.Message)
                        ? "Cambio de asignación SSOMA procesado correctamente."
                        : changeResult.Message,
                    Id = targetOperationsTeamSsomaId
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static Core.Projections.Operations.OperationsTeamSsomaAssignmentItem BuildUpdatedItem(
            Core.Projections.Operations.OperationsTeamSsomaAssignmentItem beforeItem,
            ProcessSsomaAssignmentChangeDto dto)
        {
            return new Core.Projections.Operations.OperationsTeamSsomaAssignmentItem
            {
                OperationsTeamSsomaId = beforeItem.OperationsTeamSsomaId,
                WorkerId = dto.ChangeType == SsomaAssignmentChangeType.Replacement ? dto.WorkerId : beforeItem.WorkerId,
                SsomaRoleId = dto.SsomaRoleId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsPrimary = dto.IsPrimary
            };
        }

        private static long? ResolveTargetSsomaProcessId(
            ProcessSsomaAssignmentChangeDto dto,
            long sourceSsomaProcessId)
        {
            if (dto.ChangeType == SsomaAssignmentChangeType.Update)
                return dto.ToSsomaProcessId ?? sourceSsomaProcessId;

            return dto.ToSsomaProcessId ?? dto.SsomaProcessId;
        }

        private static void ValidateBusinessRules(
            ProcessSsomaAssignmentChangeDto dto,
            Core.Projections.Operations.OperationsTeamSsomaDetailProjection before,
            Core.Projections.Operations.OperationsTeamSsomaAssignmentItem beforeItem,
            long sourceSsomaProcessId)
        {
            switch (dto.ChangeType)
            {
                case SsomaAssignmentChangeType.Update:
                    if (dto.ToSsomaProcessId.HasValue && dto.ToSsomaProcessId.Value != sourceSsomaProcessId)
                        throw new InvalidOperationException("Una actualización no puede cambiar el proceso SSOMA.");

                    if (dto.WorkerId.HasValue && dto.WorkerId.Value != beforeItem.WorkerId)
                        throw new InvalidOperationException("Una actualización no puede cambiar el trabajador asignado.");

                    break;

                case SsomaAssignmentChangeType.Relocation:
                    if (!before.IsActive)
                        throw new InvalidOperationException("No se puede reubicar una asignación SSOMA inactiva.");

                    break;

                case SsomaAssignmentChangeType.Replacement:
                    //if (!before.IsActive)
                    //    throw new InvalidOperationException("No se puede reemplazar una asignación SSOMA inactiva.");

                    if (!dto.WorkerId.HasValue)
                        throw new InvalidOperationException("El trabajador es obligatorio para un reemplazo.");

                    if (beforeItem.WorkerId.HasValue && beforeItem.WorkerId.Value == dto.WorkerId.Value)
                        throw new InvalidOperationException("El trabajador de reemplazo debe ser distinto al trabajador actual.");

                    if (dto.ReplacedAssignmentId.HasValue && dto.ReplacedAssignmentId.Value == dto.OperationsTeamSsomaId)
                        throw new InvalidOperationException("La asignación reemplazada no puede ser la misma asignación origen.");

                    break;
            }
        }
    }
}
