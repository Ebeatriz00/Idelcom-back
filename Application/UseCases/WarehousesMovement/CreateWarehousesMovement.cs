using Application.DTOs.WarehousesMovement;
using Application.Exceptions;
using AutoMapper;
using Core.Entities.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.WarehousesMovement
{
    public class CreateWarehousesMovement(
        IWarehousesMovement repository,
        WarehousesMovementStockService stockService,
        WarehousesMovementBusinessRules businessRules,
        IValidator<WarehousesMovementCreateDto> validator,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IWarehousesMovement _repository = repository;
        private readonly WarehousesMovementStockService _stockService = stockService;
        private readonly WarehousesMovementBusinessRules _businessRules = businessRules;
        private readonly IValidator<WarehousesMovementCreateDto> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponseId> ExecuteAsync(WarehousesMovementCreateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errors);
            }

            var movementResolution = await _businessRules.ValidateForCreateAsync(dto, businessId);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<Core.Entities.Logistic.WarehousesMovement>(dto);
                entity.BusinessId = businessId;
                entity.CreateUser = userId;

                entity.MovementDate ??= DateTime.Now;

                var details = dto.Details
                    .Select(detailDto =>
                    {
                        var detail = _mapper.Map<WarehouseMovementDetail>(detailDto);
                        detail.BusinessId = businessId;
                        detail.CreateUser = userId;
                        detail.TotalCost = detail.Quantity * detail.UnitCost;
                        return detail;
                    })
                    .ToList();

                var created = await _repository.AddAsync(entity, details, transaction);

                if (created.Id is null or <= 0)
                    throw new DatabaseException("Error al registrar el movimiento de almacen.", "No se obtuvo el id creado.");

                entity.WarehouseMovementId = (long)created.Id;

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.WarehousesMovement,
                    entity.WarehouseMovementId,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

                foreach (var detail in details)
                {
                    detail.WarehouseMovementId = entity.WarehouseMovementId;

                    var createdDetail = await _repository.AddDetailAsync(detail, transaction);
                    if (createdDetail.Id is null or <= 0)
                        throw new DatabaseException("Error al registrar el detalle del movimiento de almacen.", "No se obtuvo el id creado.");

                    detail.WarehouseMovementDetailId = (long)createdDetail.Id;

                    var detailAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.WarehousesMovementDetail,
                        detail.WarehouseMovementDetailId,
                        userId);

                    await _auditService.RegisterCreateAsync(detail, detailAuditLog, transaction);
                    await _stockService.ApplyAsync(movementResolution, entity, detail, userId, transaction);
                }

                transaction.Commit();
                return created;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al registrar el movimiento de almacen en base de datos.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

    }
}
