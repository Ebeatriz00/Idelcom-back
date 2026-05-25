using Application.DTOs.ProductTypes;
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

namespace Application.UseCases.ProductTypes
{
    public class CreateProductTypes(
        IProductTypesRepository repository,
        IValidator<ProductTypesCreateDto> validator,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IProductTypesRepository _repository = repository;
        private readonly IValidator<ProductTypesCreateDto> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponseId> ExecuteAsync(ProductTypesCreateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, businessId))
                throw new DuplicateEntryException("El tipo de producto ya existe para este negocio.");

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<Core.Entities.Logistic.ProductTypes>(dto);

                entity.CreateUser = userId;
                entity.BusinessId = businessId;

                var created = await _repository.AddAsync(entity, transaction);

                if (created.Id is null)
                    throw new DatabaseException("Error al registrar el tipo de producto.", "No se obtuvo el id creado.");

                entity.ProductTypesId = (long)created.Id;
                entity.BusinessId = (long)businessId;

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.ProductTypes,
                    (long)created.Id,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

                transaction.Commit();
                return created;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al registrar el tipo de producto en base de datos.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
