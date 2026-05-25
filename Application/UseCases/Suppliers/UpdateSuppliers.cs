using Application.DTOs.Suppliers;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Suppliers
{
    public class UpdateSuppliers
    {
        private readonly ISuppliersRepository _repository;
        private readonly IValidator<SuppliersUpdateDto> _validator;
        private readonly IMapper _mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IAuditService _auditService;
        private readonly IAuditLogFactory _auditLogFactory;

        public UpdateSuppliers(
            ISuppliersRepository repository,
            IValidator<SuppliersUpdateDto> validator,
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

        public async Task<BaseResponse> ExecuteAsync(SuppliersUpdateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.DocumentNumber!, dto.SupplierName!, businessId, dto.SuppliersId))
                throw new DuplicateEntryException("El proveedor ya existe para este negocio.");

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(dto.SuppliersId, transaction);
                if (before is null)
                    throw new NotFoundException("No se encontro el proveedor.", dto.SuppliersId);

                var entity = _mapper.Map<Core.Entities.Logistic.Suppliers>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);

                var after = await _repository.GetByIdAsync(dto.SuppliersId, transaction);
                if (after is null)
                    throw new NotFoundException("No se encontro el proveedor actualizado.", dto.SuppliersId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.Suppliers,
                    dto.SuppliersId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();
                return updated;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
