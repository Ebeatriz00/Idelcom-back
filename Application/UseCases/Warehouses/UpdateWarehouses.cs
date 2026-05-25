using Application.DTOs.Warehouses;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Warehouses
{
    public class UpdateWarehouses(
        IWarehousesRepository repository,
        IValidator<WarehousesUpdateDto> validator,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IWarehousesRepository _repository = repository;
        private readonly IValidator<WarehousesUpdateDto> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponse> ExecuteAsync(WarehousesUpdateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, userId, dto.WarehousesId))
            {
                throw new DuplicateEntryException("El almacén ya existe para este negocio.");
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(dto.WarehousesId, transaction);
                if (before is null)
                    throw new NotFoundException("No se encontró el almacén.", dto.WarehousesId);

                var entity = _mapper.Map<Core.Entities.Logistic.Warehouses>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);

                var after = await _repository.GetByIdAsync(dto.WarehousesId, transaction);
                if (after is null)
                    throw new NotFoundException("No se encontró el almacén actualizado.", dto.WarehousesId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.Warehouses,
                    dto.WarehousesId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();

                return updated;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al actualizar el almacén en base de datos.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
