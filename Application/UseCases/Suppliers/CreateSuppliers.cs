using Application.DTOs.Suppliers;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Suppliers
{
    public class CreateSuppliers
    {
        private readonly ISuppliersRepository _repository;
        private readonly IValidator<SuppliersCreateDto> _validator;
        private readonly IMapper _mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IAuditService _auditService;
        private readonly IAuditLogFactory _auditLogFactory;

        public CreateSuppliers(
            ISuppliersRepository repository,
            IValidator<SuppliersCreateDto> validator,
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

        public async Task<BaseResponseId> ExecuteAsync(SuppliersCreateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.DocumentNumber!, dto.SupplierName!, businessId);
            if (yaExiste)
                throw new DuplicateEntryException("El proveedor ya existe para este negocio.");

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = _mapper.Map<Core.Entities.Logistic.Suppliers>(dto);
                entity.BusinessId = businessId;
                entity.CreateUser = userId;

                var created = await _repository.AddAsync(entity, transaction);
                if (created.Id is null)
                    throw new DatabaseException("Error al registrar el proveedor.", "No se obtuvo el id creado.");

                entity.SuppliersId = (long)created.Id;

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.Suppliers,
                    (long)created.Id,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

                transaction.Commit();
                return created;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
