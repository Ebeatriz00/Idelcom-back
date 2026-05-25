using Application.DTOs.ProductTypes;
using Application.Exceptions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.ProductTypes
{
    public class UpdateProductTypes(
        IProductTypesRepository repository,
        IValidator<ProductTypesUpdateDto> validator,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IProductTypesRepository _repository = repository;
        private readonly IValidator<ProductTypesUpdateDto> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponse> ExecuteAsync(ProductTypesUpdateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, businessId, dto.ProductTypesId))
                throw new DuplicateEntryException("El tipo de producto ya existe para este negocio.");

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(dto.ProductTypesId, transaction);
                if (before is null)
                    throw new NotFoundException("No se encontró el tipo de producto.", dto.ProductTypesId);

                var entity = _mapper.Map<Core.Entities.Logistic.ProductTypes>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);

                var after = await _repository.GetByIdAsync(dto.ProductTypesId, transaction);
                if (after is null)
                    throw new NotFoundException("No se encontró el tipo de producto actualizado.", dto.ProductTypesId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.ProductTypes,
                    dto.ProductTypesId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();

                return updated;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al actualizar el tipo de producto en base de datos.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
