using Application.DTOs.SsomaProcess;
using Application.Exceptions;
using AutoMapper;
using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.SsomaProcess
{
    public class UpdateSsomaProcess(
        ISsomaProcessRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaProcessUpdateDto> validator)
    {
        private readonly ISsomaProcessRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaProcessUpdateDto> _validator = validator;

        public async Task<GlobalResponse> ExecuteAsync(SsomaProcessUpdateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var before = await _repository.GetByIdAsync(dto.SsomaProcessId, dto.OperationsId, businessId);
                if (before == null)
                    throw new Exception("No se encontró el proceso ssoma.");

                var entity = _mapper.Map<Core.Entities.Ssoma.SsomaProcess>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;
                var updated = await _repository.UpdateAsync(entity,  transaction);

                var after = await _repository.GetByIdAsync(dto.SsomaProcessId, dto.OperationsId, businessId, transaction);

                if (after == null)
                    throw new NotFoundException("No se encontro procesos ssoma", dto.SsomaProcessId);

                var audilog = _auditLogFactory.Create(
                    businessId,
                    TableNames.SsomaProcess,
                    before.SsomaProcessId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, audilog, transaction);

                transaction.Commit();
                return updated;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actulizar procesos SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar procesos SSOMA.", ex.Message);
            }
        }
    }
}
