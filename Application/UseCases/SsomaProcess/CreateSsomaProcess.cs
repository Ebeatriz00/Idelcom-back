using Application.DTOs.SsomaProcess;
using AutoMapper;
using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using SharedKernel.Constants;
using System.Data;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.SsomaProcess
{
    public class CreateSsomaProcess(
        ISsomaProcessRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaProcessCreateDto> validator
        )
    {
        private readonly ISsomaProcessRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaProcessCreateDto> _validator = validator;
        public async Task<GlobalResponse> ExecuteAsync(SsomaProcessCreateDto dto, long userId, long businessId)
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
                var entity = _mapper.Map<Core.Entities.Ssoma.SsomaProcess>(dto);

                entity.CreateUser = userId;
                entity.BusinessId = businessId;

                var created = await _repository.AddAsync(entity, transaction);

                if (created.Id == null)
                {
                    throw new Exception("No se pudo crear el proceso SSOMA.");
                }

                entity.OperationsId = (long)created.Id;
                entity.BusinessId = (long)businessId;


                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.SsomaProcess,
                    (long)created.Id,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, trans: transaction);

                transaction.Commit();
                return created;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar procesos SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar processos SSOMA.", ex.Message);
            }
        }
    }
    
}
