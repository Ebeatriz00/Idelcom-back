using Application.DTOs.Warehouses;
using Application.Exceptions;
using Application.Services.Audit;
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
    public class CreateWarehouses(
        IWarehousesRepository repository,
        IValidator<WarehousesCreateDto> validator,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IWarehousesRepository _repository = repository;
        private readonly IValidator<WarehousesCreateDto> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        
        public async Task<BaseResponseId> ExecuteAsync(WarehousesCreateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.Description, businessId);
            if (yaExiste)
                throw new DuplicateEntryException("El almacén ya existe para este negocio.");

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();


            try
            {
                var entity = _mapper.Map<Core.Entities.Logistic.Warehouses>(dto);

                entity.CreateUser = userId;
                entity.BusinessId = businessId;

                var created = await _repository.AddAsync(entity, transaction);

                if (created.Id is null)
                    throw new DatabaseException("Error al registrar el almacén.", "No se obtuvo el id creado.");

                entity.WarehousesId = (long)created.Id;
                entity.BusinessId = (long)businessId;

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.Warehouses,
                    (long)created.Id,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

                transaction.Commit();
                return created;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al registrar el almacén en base de datos.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
