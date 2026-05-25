using Application.DTOs.FileTrackingProducts;
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


namespace Application.UseCases.FileTrackingProducts
{
    public class CreateProductFile
    {
        private readonly IFileTrackingProductsRepository _repository;
        private readonly IValidator<FileTrackingProductsCreateDto> _validator;
        private readonly IMapper _mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IAuditService _auditService;
        private readonly IAuditLogFactory _auditLogFactory;

        public CreateProductFile(
            IFileTrackingProductsRepository repository,
            IValidator<FileTrackingProductsCreateDto> validator,
            IMapper mapper,
            ISqlConnectionFactory sqlConnectionFactory,
            IAuditService auditService,
            IAuditLogFactory auditLogFactory)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _sqlConnectionFactory = sqlConnectionFactory;
            _auditService = auditService;
            _auditLogFactory = auditLogFactory;
        }

        public async Task<BaseResponseId> ExecuteAsync(FileTrackingProductsCreateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(businessId, dto.ProductsId, dto.FileTitle))
                throw new DuplicateEntryException("La imagen ya existe para este negocio.");

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<Core.Entities.Logistic.FileTrackingProducts>(dto);

                entity.CreateUser = userId;
                entity.BusinessId = businessId;

                var created = await _repository.AddAsync(entity, transaction);

                if (created.Id is null)
                    throw new DatabaseException("Error al registrar la marca.", "No se obtuvo el id creado.");

                entity.FileTrackingProductsId = (long)created.Id;
                entity.BusinessId = (long)businessId;

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.FileTrackingProducts,
                    (long)created.Id,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

                transaction.Commit();
                return created;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al registrar la marca en base de datos.", ex.Message);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }
    }
}
