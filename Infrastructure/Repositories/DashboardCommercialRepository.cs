using Core.Entities;
using Core.Interfaces;
using DocumentFormat.OpenXml.Bibliography;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DashboardCommercialRepository : IDashboardCommercialRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public DashboardCommercialRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<StateOpportunityMetric>> GetMetricsByStateOpportunity(long? usersId, int? quarter, long? usersBy, int? year)
        {
            try
            {
                var metrics = new List<StateOpportunityMetric>();

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_METRICS_BY_STATE", connection)
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
                    metrics.Add(new StateOpportunityMetric
                    {
                        StateOpportunityId = reader.GetInt32(reader.GetOrdinal("STATE_OPPORTUNITY_ID")),
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


        public async Task<IEnumerable<ClientMetric>> GetMetricsClients(long? usersId, int? quarter, long? usersBy, int? year)
        {
            try
            {
                var metrics = new List<ClientMetric>();
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_DASHBOARD_METRICS_CLIENTS", connection)
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
                    metrics.Add(new ClientMetric
                    {
                        Quantity = Convert.ToInt32(reader["QUANTITY"])
                    });
                }

                return metrics;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener métricas de clientes.", ex);
            }
        }

        public async Task<IEnumerable<QuarterMetric>> GetMetricsByQuarter(long? usersId, int? quarter, long? usersBy, int? year)
        {
            var metrics = new List<QuarterMetric>();
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("SP_DASHBOARD_METRICS_BY_QUARTER", connection)
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
                metrics.Add(new QuarterMetric
                {
                    QuarterNum = Convert.ToInt32(reader["QUARTER_NUM"]),
                    Quantity = Convert.ToInt32(reader["QUANTITY"])
                });
            }
            return metrics;
        }

        public async Task<IEnumerable<CombinedMetric>> GetMetricsCombined(long? usersId, int? quarter, long? usersBy, int? year)
        {
            var metrics = new List<CombinedMetric>();
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("SP_DASHBOARD_METRICS_COMBINED", connection)
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
                metrics.Add(new CombinedMetric
                {
                    QuarterNum = Convert.ToInt32(reader["QUARTER_NUM"]),
                    StateName = reader["STATE_NAME"].ToString(),
                    StateColor = reader["STATE_COLOR"].ToString(),
                    Quantity = Convert.ToInt32(reader["QUANTITY"])
                });
            }
            return metrics;
        }

        public async Task<IEnumerable<DashboardCommercialProbability>> GetDashboardProbabilityAmount(long? usersId, int? quarter, long? usersBy, decimal? probability, int? year)
        {
            try
            {
                var metrics = new List<DashboardCommercialProbability>();

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_WS_DASHBOARD_COMMERCIAL_PROBABILITY", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@PROBABILITY", (object?)probability ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardCommercialProbability
                    {
                        StateOpportunityId = Convert.ToInt64(reader["STATE_OPPORTUNITY_ID"]),
                        PorcentProgressPro = Convert.ToInt32(reader["PORCENT_PROGRESS_PRO"]),
                        TotalAmount = Convert.ToDecimal(reader["TOTAL_AMOUNT"]),
                        StateDesc = reader["STATE_DESC"].ToString(),
                        StateColor = reader["STATE_COLOR"].ToString()
                    });
                }

                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener las métricas de probabilidad y monto.", ex.Message);
            }
        }

        public async Task<IEnumerable<DashboardCommercialEvolution>> GetDashboardCommercialEvolution(long? usersId, int? year, int? quarter, long? usersBy)
        {
            try
            {
                var metrics = new List<DashboardCommercialEvolution>();

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_WS_DASHBOARD_COMMERCIAL_EVOLUTION", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value); 
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardCommercialEvolution
                    {
                        Year = reader.GetInt32(reader.GetOrdinal("Year")),
                        Month = reader.GetInt32(reader.GetOrdinal("Month")),
                        StateOpportunityId = reader.GetInt64(reader.GetOrdinal("StateOpportunityId")),
                        StateName = reader.GetString(reader.GetOrdinal("StateName")), 
                        StateColor = reader.GetString(reader.GetOrdinal("StateColor")), 
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
                    });
                }

                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving commercial evolution metrics.", ex.Message);
            }
        }


        public async Task<IEnumerable<DashboardCommercialClosing>> GetDashboardClosingMetrics(long? usersId, int? year, int? quarter, long? usersBy)
        {
            try
            {
                var metrics = new List<DashboardCommercialClosing>();

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_WS_DASHBOARD_COMMERCIAL_CLOSING", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardCommercialClosing
                    {
                        Year = reader.GetInt32(reader.GetOrdinal("Year")),
                        Month = reader.GetInt32(reader.GetOrdinal("Month")),
                        StateOpportunityId = reader.GetInt64(reader.GetOrdinal("StateOpportunityId")), 
                        StateName = reader.GetString(reader.GetOrdinal("StateName")),
                        StateColor = reader.GetString(reader.GetOrdinal("StateColor")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
                    });
                }

                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving closing metrics.", ex.Message);
            }
        }


        public async Task<IEnumerable<DashboardCommercialClient>> GetDashboardClientOpportunity(long? usersId, int? year, int? quarter, long? usersBy, long? clientId = null)
        {
            try
            {
                var metrics = new List<DashboardCommercialClient>();

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_WS_DASHBOARD_COMMERCIAL_BY_CLIENT", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                command.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);
                command.Parameters.AddWithValue("@YEAR", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@QUARTER", (object?)quarter ?? DBNull.Value);
                command.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@CLIENTS_ID", (object?)clientId ?? DBNull.Value);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    metrics.Add(new DashboardCommercialClient
                    {
                        ClientId = reader.GetInt64(reader.GetOrdinal("ClientId")),
                        ClientName = reader.GetString(reader.GetOrdinal("ClientName")),
                        StateOpportunityId = reader.GetInt64(reader.GetOrdinal("StateOpportunityId")),
                        StateName = reader.GetString(reader.GetOrdinal("StateName")),
                        StateColor = reader.GetString(reader.GetOrdinal("StateColor")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
                    });
                }

                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving client opportunity metrics.", ex.Message);
            }
        }


        public async Task<IEnumerable<DashboardCommercialTotals>> GetDashboardTotals(long? usersId, int? quarter, long? usersBy, int? year)
        {
            try
            {
                var metrics = new List<DashboardCommercialTotals>();

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SP_WS_DASHBOARD_COMMERCIAL_TOTALS", connection)
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
                    metrics.Add(new DashboardCommercialTotals
                    {
                        TotalQty = reader.GetInt32(reader.GetOrdinal("TOTAL_QTY")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("TOTAL_AMOUNT")),
                        WonQty = reader.GetInt32(reader.GetOrdinal("WON_QTY")),
                        WonAmount = reader.GetDecimal(reader.GetOrdinal("WON_AMOUNT")),
                        ConversionRate = reader.GetDecimal(reader.GetOrdinal("CONVERSION_RATE")),
                    });
                }
                return metrics;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la lista.", ex.Message);
            }
        }



    }
}
