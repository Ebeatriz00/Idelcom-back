using Application.DTOs.OperationsTeamSsoma;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.OperationsTeamSsoma
{
    public class CreateOperationsTeamSsoma(
        IOperationsTeamSsomaRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsTeamSsomaCreateDto> validator)
    {
        private readonly IOperationsTeamSsomaRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsTeamSsomaCreateDto> _validator = validator;

        public async Task<GlobalResponse> ExecuteAsync(
            OperationsTeamSsomaCreateDto dto,
            long userId,
            long businessId)
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
                var entity = _mapper.Map<Core.Entities.Operations.OperationsTeamSsoma>(dto);
                entity.BusinessId = businessId;
                entity.CreateUser = userId;

                var teamSsoma = _mapper.Map<List<OperationsTeamSsomaAssignmentItem>>(dto.TeamSsoma);

                var createResult = await _repository.AddAsync(entity, teamSsoma, transaction);

                var result = createResult.Response;
                var insertedRows = createResult.InsertedRows;

                if (result.Status == 0)
                    throw new InvalidOperationException("No se pudo crear el equipo de SSOMA de operaciones.");

                if (insertedRows is not null && insertedRows.Any())
                {
                    foreach (var item in teamSsoma)
                    {
                        var inserted = insertedRows.FirstOrDefault(x =>
                            x.WorkerId == item.WorkerId &&
                            x.SsomaRoleId == item.SsomaRoleId &&
                            x.StartDate == item.StartDate);

                        if (inserted is null)
                            continue;

                        item.OperationsTeamSsomaId = inserted.OperationsTeamSsomaId;

                        var auditLog = _auditLogFactory.Create(
                            businessId,
                            TableNames.OperationsTeamSsoma,
                            inserted.OperationsTeamSsomaId,
                            userId);

                        await _auditService.RegisterCreateAsync(item, auditLog, transaction);
                    }
                }

                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
