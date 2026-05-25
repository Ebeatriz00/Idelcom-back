using Application.DTOs.Brands;
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

namespace Application.UseCases.Brands
{
    public class UpdateBrands(
        IBrandsRepository repository,
        IValidator<BrandsUpdateDto> validator,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory
        )
    {
        private readonly IBrandsRepository _repository = repository;
        private readonly IValidator<BrandsUpdateDto> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponse> ExecuteAsync(BrandsUpdateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, businessId, dto.BrandsId))
                throw new DuplicateEntryException("La marca ya existe para este negocio.");

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(dto.BrandsId, transaction);
                if (before is null)
                    throw new NotFoundException("No se encontró la marca.", dto.BrandsId);

                var entity = _mapper.Map<Core.Entities.Logistic.Brands>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);

                var after = await _repository.GetByIdAsync(dto.BrandsId, transaction);
                if (after is null)
                    throw new NotFoundException("No se encontró la marca actualizada.", dto.BrandsId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.Brands,
                    dto.BrandsId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();

                return updated;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al actualizar la marca en base de datos.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
