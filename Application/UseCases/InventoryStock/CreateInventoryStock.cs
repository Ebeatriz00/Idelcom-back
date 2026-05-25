using Application.DTOs.InventoryStock;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.InventoryStock
{
    public class CreateInventoryStock(
        IInventoryStockRepository repository,
        InventoryStockBusinessRules businessRules,
        IValidator<InventoryStockCreateDto> validator,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IInventoryStockRepository _repository = repository;
        private readonly InventoryStockBusinessRules _businessRules = businessRules;
        private readonly IValidator<InventoryStockCreateDto> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponseId> ExecuteAsync(InventoryStockCreateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errors);
            }

            await _businessRules.ValidateForCreateAsync(dto, businessId);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<Core.Entities.Logistic.InventoryStock>(dto);
                entity.BusinessId = businessId;
                entity.CreateUser = userId;

                if (entity.StockQuantity > 0 && !entity.LastEnteryDate.HasValue)
                    entity.LastEnteryDate = DateTime.Now;

                var created = await _repository.AddAsync(entity, transaction);
                if (created.Id is null or <= 0)
                    throw new DatabaseException("Error al registrar stock de inventario.", "No se obtuvo el id creado.");

                entity.InventoryStockId = (long)created.Id;

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.InventoryStock,
                    entity.InventoryStockId,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

                transaction.Commit();
                return created;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al registrar stock de inventario en base de datos.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
