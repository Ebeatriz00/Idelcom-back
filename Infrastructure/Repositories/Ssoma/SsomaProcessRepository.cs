using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Interfaces.Ssoma;
using Core.Projections.Ssoma;
using Dapper;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Ssoma
{
    public class SsomaProcessRepository(IDapperHelper dapperHelper) : ISsomaProcessRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<GlobalResponse> AddAsync(SsomaProcess entity, IDbTransaction transaction)
        {
            try
            {
             
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.OperationsId,
                    entity.RequiresCompanyHomologation,
                    entity.RequieresOperationTeamSsoma,
                    entity.CurrentStatusId,
                    entity.RequestDate,
                    entity.SubmissionsDate,
                    entity.ApprovalDate,
                    entity.StartDate,
                    entity.EndDate,
                    entity.GeneralObservation,
                    entity.CreateUser
                })

                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                var result = await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_SSOMA_PROCESS",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = parameters.Get<long>("@Id");

                if (cOutput != 1)
                    throw new DatabaseException(sOutput);

                return new GlobalResponse
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar procesos SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar procesos SSOMA.", ex.Message);
            }

        }
        public async Task<GlobalResponse> UpdateAsync(SsomaProcess entity, IDbTransaction transaction)
        {
            try
            {
              var parameters = DapperParams.From(new {
                  entity.SsomaProcessId,
                  entity.BusinessId,
                  entity.OperationsId,
                  entity.RequiresCompanyHomologation,
                  entity.RequieresOperationTeamSsoma,
                  entity.CurrentStatusId,
                  entity.RequestDate,
                  entity.SubmissionsDate,
                  entity.ApprovalDate,
                  entity.StartDate,
                  entity.EndDate,
                  entity.GeneralObservation,
                  entity.UpdateUser
              })
              .WithOutputInt("@COutput")
              .WithOutputString("@SOutput", 500);

                var result = await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_SSOMA_PROCESS",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                return new GlobalResponse
                {
                    Status = cOutput,
                    Message = sOutput
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el equipo SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el equipo SSOMA.", ex.Message);
            }
        }

        public async Task<PagedResult<SsomaProcessListItem>> GetAllAsync(long businessId, int page, int pageSize, string? search, long? operationsId = null)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<SsomaProcessListItem>(
                "SP_WS_LIST_SSOMA_PROCESS",
                parameters,
                commandType: CommandType.StoredProcedure);

            return new PagedResult<SsomaProcessListItem>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<SsomaProcess?> GetByIdAsync(long ssomaProcessId, long operationsId, long businessId)
        {
            
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                SsomaProcessId = ssomaProcessId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaProcess>(
                "SP_WS_GETBYID_SSOMA_PROCESS",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<SsomaProcess?> GetByIdAsync(long ssomaProcessId, long operationsId, long businessId, IDbTransaction transaction)
        {

            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                SsomaProcessId = ssomaProcessId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaProcess>(
                "SP_WS_GETBYID_SSOMA_PROCESS",
                parameters,
                transaction);
        }

        public async Task<GlobalResponse> DeleteAsync(long ssomaProcessId, long userId, IDbTransaction transaction)
        {
            try
            {

                var parameters = DapperParams.From(new
                {
                    SsomaProcessId = ssomaProcessId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);


                await _dapperHelper.ExecuteAsync(
                    "SP_WS_DELETE_SSOMA_PROCESS",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new GlobalResponse
                {
                    Status = cOutput,
                    Message = sOutput
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado.", ex.Message);
            }
        }

    }
}
