using Application.DTOs.Products;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Products
{
    public class UpdateProducts(
        IProductsRepository repository,
        IValidator<ProductsUpdateDto> validator,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory
        )
    {
        private readonly IProductsRepository _repository = repository;
        private readonly IValidator<ProductsUpdateDto> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

                                                                                                                                                 
        public async Task<BaseResponse> ExecuteAsync(ProductsUpdateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                         .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                         .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, businessId, dto.ProductsId))
            {
                throw new DuplicateEntryException("El producto ya existe para este negocio.");
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var before = await _repository.GetByIdAsync(dto.ProductsId, transaction);
                if (before is null)
                    throw new NotFoundException("No se encontró el producto.", dto.ProductsId);

                var entity = _mapper.Map<Core.Entities.Logistic.Products>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;
                entity.Files = before.Files;

                var updated = await _repository.UpdateAsync(entity, transaction);

                var after = await _repository.GetByIdAsync(dto.ProductsId, transaction);
                if (after is null)
                    throw new NotFoundException("No se encontró el producto actualizado.", dto.ProductsId);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.Products,
                    dto.ProductsId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();

                return updated;

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
