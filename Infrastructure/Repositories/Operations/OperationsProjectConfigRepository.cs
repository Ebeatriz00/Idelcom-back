using Core.Entities.Operations;
using Core.Interfaces.Operations;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsProjectConfigRepository(IDapperHelper dapperHelper) : IOperationsProjectConfigRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<GlobalResponse> AddAsync(OperationsProjectConfig entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.OperationsId,
                    EntryTime = DbParam.Time(entity.EntryTime),
                    DepartureTime = DbParam.Time(entity.DepartureTime),
                    entity.AllowDelay,
                    entity.MinutesTolerance,
                    BeforeOfficialTime = DbParam.Time(entity.BeforeOfficialTime),
                    entity.IsRequirePhoto,
                    entity.IsRequireOvertime,
                    entity.IsRequireOvertimeApproval,
                    entity.Shift,
                    entity.IsRequireAppAttendance,
                    entity.IsRequireGroupPhoto,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@cOutput")
                .WithOutputString("@sOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_OPERATIONS_PROJECT_CONFIG",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@cOutput");
                var sOutput = parameters.Get<string>("@sOutput");
                var id = parameters.Get<long>("@Id");

                return new GlobalResponse
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la config de proyectos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar la config de proyecto.", ex.Message);
            }
        }

        public async Task<IEnumerable<OperationsProjectConfig>> GetAllAsync(long operationsId)
        {
            var parameters = DapperParams.From(new
            {
                OperationsId = operationsId
            });

            return await _dapperHelper.QueryAsync<OperationsProjectConfig>(
                "SP_WS_GETBYID_OPERATIONS_PROJECT_CONFIG", 
                parameters);
        }

        public async Task<OperationsProjectConfig?> GetByIdAsync(long operationsProjectConfigId, long operationsId)
        {
            var parameters = DapperParams.From(new
            {
                OperationsProjectConfigId = operationsProjectConfigId,
                OperationsId = operationsId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationsProjectConfig>(
                "SP_WS_GETBYID_OPERATIONS_PROJECT_CONFIG",
                parameters);
        }
        public async Task<OperationsProjectConfig?> GetByIdAsync(long operationsProjectConfigId, long operationsId, IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                OperationsProjectConfigId = operationsProjectConfigId,
                OperationsId = operationsId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationsProjectConfig>(
                "SP_WS_GETBYID_OPERATIONS_PROJECT_CONFIG",
                parameters,
                transaction);
        }

        public async Task<GlobalResponse> UpdateAsync(OperationsProjectConfig entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.OperationsProjectConfigId,
                    entity.BusinessId,
                    entity.OperationsId,
                    EntryTime = DbParam.Time(entity.EntryTime),
                    DepartureTime = DbParam.Time(entity.DepartureTime),
                    entity.AllowDelay,
                    entity.MinutesTolerance,
                    BeforeOfficialTime = DbParam.Time(entity.BeforeOfficialTime),
                    entity.IsRequirePhoto,
                    entity.IsRequireOvertime,
                    entity.IsRequireOvertimeApproval,
                    entity.Shift,
                    entity.IsRequireAppAttendance,
                    entity.IsRequireGroupPhoto,
                    UpdateUser = entity.UpdateUser
                })
                .WithOutputInt("@cOutput")
                .WithOutputString("@sOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_OPERATIONS_PROJECT_CONFIG",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@cOutput");
                var sOutput = parameters.Get<string>("@sOutput");

                return new GlobalResponse
                {
                    Status = cOutput,
                    Message = sOutput
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la config de proyectos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar la config de proyecto.", ex.Message);
            }
        }
    }
}
