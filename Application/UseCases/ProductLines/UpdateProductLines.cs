using Application.DTOs.ProductLines;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.logistic;
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

namespace Application.UseCases.ProductLines
{
    public class UpdateProductLines
    {
        private readonly IProductLinesRepository _repository;
        private readonly IValidator<ProductLinesUpdateDto> _validator;
        private readonly IMapper _mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IAuditService _auditService;
        private readonly IAuditLogFactory _auditLogFactory;

        public UpdateProductLines(IProductLinesRepository repository, IValidator<ProductLinesUpdateDto> validator, IMapper mapper, ISqlConnectionFactory sqlConnectionFactory, IAuditService auditService, IAuditLogFactory auditLogFactory)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _sqlConnectionFactory = sqlConnectionFactory;
            _auditService = auditService;
            _auditLogFactory = auditLogFactory;
        }
        

        public async Task<BaseResponse> ExecuteAsync(ProductLinesUpdateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, businessId, dto.ProductLinesId))
                throw new DuplicateEntryException("La línea de producto ya existe para este negocio.");

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(dto.ProductLinesId, transaction);
                if (before is null)
                    throw new NotFoundException("No se encontró la línea de producto.", dto.ProductLinesId);

                var entity = _mapper.Map<Core.Entities.Logistic.ProductLines>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);

                var after = await _repository.GetByIdAsync(dto.ProductLinesId, transaction);
                if (after is null)
                    throw new NotFoundException("No se encontró la línea de producto actualizada.", dto.ProductLinesId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.ProductLines,
                    dto.ProductLinesId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();

                return updated;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al actualizar la línea de producto en base de datos.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
