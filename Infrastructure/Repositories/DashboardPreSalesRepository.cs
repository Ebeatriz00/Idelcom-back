using Core.Entities;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DashboardPreSalesRepository : IDashboardPreSalesRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public DashboardPreSalesRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<DashboardPreSalesQuotation>> GetTotalQuotation(long? usersId, int? quarter, long? usersBy, int? year)
        {
            try
            {
                var metrics = new List<DashboardPreSalesQuotation>();

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_WS_DASHBOARD_PRESALES_QUOTATIONS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardPreSalesQuotation
                    {
                        Quantity = reader.GetInt32(reader.GetOrdinal("QUANTITY"))
                    });
                }

                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }
        }


        public async Task<IEnumerable<DashboardPreSalesState>> GetDashboardPreSalesState(long? usersId, int? quarter, long? usersBy, int? year)
        {
            try
            {
                var metrics = new List<DashboardPreSalesState>();

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_METRICS_BY_STATE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardPreSalesState
                    {
                        StatePreSaleId = reader.GetInt64(reader.GetOrdinal("STATE_PRE_SALE_ID")),
                        StateName = reader.GetString(reader.GetOrdinal("STATE_NAME")),
                        StateColor = reader.GetString(reader.GetOrdinal("STATE_COLOR")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("QUANTITY"))
                    });
                }

                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }
        }


        public async Task<IEnumerable<DashboardPreSalesCombined>> GetDashboardPreSalesCombined(long? usersId, int? quarter, long? usersBy, int? year)
        {
            try
            {
                var metrics = new List<DashboardPreSalesCombined>();

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_METRICS_COMBINED", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardPreSalesCombined
                    {
                        QuarterNum = reader.GetInt32(reader.GetOrdinal("QUARTER_NUM")),
                        StateName = reader.GetString(reader.GetOrdinal("STATE_NAME")),
                        StateColor = reader.GetString(reader.GetOrdinal("STATE_COLOR")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("QUANTITY"))
                    });
                }

                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }
        }
        public async Task<IEnumerable<DashboardPreSalesByEngineer>> GetDashboardPreSalesByEngineer(long? usersId, int? quarter, int? year, long? stateId)
        {
            try
            {
                var metrics = new List<DashboardPreSalesByEngineer>();
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_BY_ENGINEER_METRICS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@STATE_ID", (object?)stateId ?? DBNull.Value);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardPreSalesByEngineer
                    {
                        Responsible = reader.GetString(reader.GetOrdinal("RESPONSIBLE")),
                        TotalVersions = reader.GetInt32(reader.GetOrdinal("TOTAL_VERSIONS")),
                        GeneralAmount = reader.GetDecimal(reader.GetOrdinal("GENERAL_AMOUNT")),
                        ClosedAmount = reader.GetDecimal(reader.GetOrdinal("CLOSED_AMOUNT"))
                    });
                }
                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }
        }

        public async Task<IEnumerable<DashboardPreSalesMatriz>> GetDashboardPreSalesMatriz(long? usersId, int? quarter, long? usersBy, int? year)
        {
            try
            {
                var metrics = new List<DashboardPreSalesMatriz>();
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_COMMERCIAL_MATRIZ", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardPreSalesMatriz
                    {
                        WorkerId = reader.GetInt64(reader.GetOrdinal("WORKER_ID")),
                        WorkerName = reader.GetString(reader.GetOrdinal("WORKER_NAME")),
                        MonthNum = reader.GetInt32(reader.GetOrdinal("MONTH_NUM")),
                        WonAmount = reader.GetDecimal(reader.GetOrdinal("WON_AMOUNT")),
                        GeneralAmount = reader.GetDecimal(reader.GetOrdinal("GENERAL_AMOUNT")),
                        TotalQuotations = reader.GetInt32(reader.GetOrdinal("TOTAL_QUOTATIONS"))
                    });
                }
                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }
        }

        public async Task<IEnumerable<DashboardPreSalesCollaborator>> GetDashboardPreSalesCollaborator(long? usersId, int? quarter, int? year, long? stateId)
        {
            try
            {
                var metrics = new List<DashboardPreSalesCollaborator>();
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_COLLABORATOR_METRICS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@STATE_ID", (object?)stateId ?? DBNull.Value);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardPreSalesCollaborator
                    {
                        CollaboratorName = reader.GetString(reader.GetOrdinal("COLLABORATOR_NAME")),
                        TotalVersions = reader.GetInt32(reader.GetOrdinal("TOTAL_VERSIONS")),
                        GeneralAmount = reader.GetDecimal(reader.GetOrdinal("GENERAL_AMOUNT")),
                        ClosedAmount = reader.GetDecimal(reader.GetOrdinal("CLOSED_AMOUNT"))
                    });
                }
                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }


        }

        public async Task<IEnumerable<DashboardPreSalesIntegrators>> GetDashboardPreSalesIntegrators(long? usersId, int? quarter, int? year, long? stateId)
        {
            try
            {
                var metrics = new List<DashboardPreSalesIntegrators>();
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_BY_INTEGRATOR_METRICS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@STATE_ID", (object?)stateId ?? DBNull.Value);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardPreSalesIntegrators
                    {
                        Integrators = reader.GetString(reader.GetOrdinal("INTEGRATORS")),
                        TotalVersions = reader.GetInt32(reader.GetOrdinal("TOTAL_VERSIONS")),
                        GeneralAmount = reader.GetDecimal(reader.GetOrdinal("GENERAL_AMOUNT")),
                        ClosedAmount = reader.GetDecimal(reader.GetOrdinal("CLOSED_AMOUNT"))
                    });
                }
                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }

        }

        public async Task<IEnumerable<DashboardPreSaleByEngineerDetails>> GetDashboardPreSalesByEngineerDetails(long? usersId, int? quarter, int? year, long? stateId)
        {
            try
            {
                var details = new List<DashboardPreSaleByEngineerDetails>();
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_BY_ENGINEER_DETAILS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@STATE_ID", (object?)stateId ?? DBNull.Value);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    details.Add(new DashboardPreSaleByEngineerDetails
                    {
                        Responsible = reader.GetString(reader.GetOrdinal("RESPONSIBLE")),
                        OpporNum = reader.GetString(reader.GetOrdinal("OPPOR_NUM")),
                        OpporDesc = reader.GetString(reader.GetOrdinal("OPPOR_DESC")),
                        GeneralAmount = reader.GetDecimal(reader.GetOrdinal("GENERAL_AMOUNT")),
                        Category = reader.GetInt32(reader.GetOrdinal("PROJECT_CATEGORY"))
                    });
                }
                return details;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }
        }

        public async Task<IEnumerable<DashboardPreSalesIntegratorsDetails>> GetDashboardPreSalesIntegratorsDetails(long? usersId, int? quarter, int? year, long? stateId)
        {
            try
            {
                var details = new List<DashboardPreSalesIntegratorsDetails>();
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_BY_INTEGRATOR_DETAILS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@STATE_ID", (object?)stateId ?? DBNull.Value);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    details.Add(new DashboardPreSalesIntegratorsDetails
                    {
                        Integrators = reader.GetString(reader.GetOrdinal("INTEGRATORS")),
                        OpporNum = reader.GetString(reader.GetOrdinal("OPPOR_NUM")),
                        OpporDesc = reader.GetString(reader.GetOrdinal("OPPOR_DESC")),
                        GeneralAmount = reader.GetDecimal(reader.GetOrdinal("GENERAL_AMOUNT")),
                        Category = reader.GetInt32(reader.GetOrdinal("PROJECT_CATEGORY"))
                    });
                }
                return details;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }
        }

        public async Task<IEnumerable<DashboardPreSalesCollaboratorDetails>> GetDashboardPreSalesCollaboratorDetails(long? usersId, int? quarter, int? year, long? stateId)
        {
            try
            {
                var details = new List<DashboardPreSalesCollaboratorDetails>();
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_BY_COLLABORATORS_DETAILS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@STATE_ID", (object?)stateId ?? DBNull.Value);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    details.Add(new DashboardPreSalesCollaboratorDetails
                    {
                        Collaborators = reader.GetString(reader.GetOrdinal("COLLABORATORS")),
                        OpporNum = reader.GetString(reader.GetOrdinal("OPPOR_NUM")),
                        OpporDesc = reader.GetString(reader.GetOrdinal("OPPOR_DESC")),
                        GeneralAmount = reader.GetDecimal(reader.GetOrdinal("GENERAL_AMOUNT")),
                        Category = reader.GetInt32(reader.GetOrdinal("PROJECT_CATEGORY"))
                    });
                }
                return details;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }
        }

        public async Task<IEnumerable<DashboardPreSalesByCategory>> GetDashboardPreSalesByCategory(long? usersId, int? quarter, int? year)
        {
            try
            {
                var metrics = new List<DashboardPreSalesByCategory>();
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_PRESALES_BY_CATEGORY", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardPreSalesByCategory
                    {
                        CategoryName = reader.GetString(reader.GetOrdinal("CATEGORY_NAME")),
                        ProjectQuantity = reader.GetInt32(reader.GetOrdinal("PROJECT_QUANTITY")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("TOTAL_AMOUNT")),
                        WonAmount = reader.GetDecimal(reader.GetOrdinal("WON_AMOUNT"))
                    });
                }
                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas del dashboard por estado.", ex.Message);
            }
        }
    }
}