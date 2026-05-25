using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Notifications
{
    public class OpportunityAlertsWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OpportunityAlertsWorker> _logger;

        private const int BatchSize = 200;

        public OpportunityAlertsWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<OpportunityAlertsWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker ALERTS iniciado ⏰");

            await Task.Yield();
            await DelayOrStopAsync(TimeSpan.FromMilliseconds(1500), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var cnFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();

                    using var cn = cnFactory.CreateConnection();
                    await cn.OpenAsync(stoppingToken);

                    // 1️ GENERAR alertas por INACTIVIDAD
                    using (var cmdInactivity = new SqlCommand("dbo.SP_WS_PROCESS_INACTIVITY_ALERTS", cn))
                    {
                        cmdInactivity.CommandType = CommandType.StoredProcedure;
                        cmdInactivity.Parameters.AddWithValue("@BUSINESS_ID", 1);

                        await cmdInactivity.ExecuteNonQueryAsync(stoppingToken);
                    }

                    using (var cmdPresales = new SqlCommand("dbo.SP_WS_PROCESS_PRESALES_ALERTS", cn))
                    {
                        cmdPresales.CommandType = CommandType.StoredProcedure;
                        cmdPresales.Parameters.AddWithValue("@BUSINESS_ID", 1);
                        await cmdPresales.ExecuteNonQueryAsync(stoppingToken);
                    }

                    // 2️ DISPARAR alertas (followups, obs, inactividad, etc)
                    using (var cmdAlerts = new SqlCommand("dbo.SP_WS_PROCESS_OPPOR_ALERTS", cn))
                    {
                        cmdAlerts.CommandType = CommandType.StoredProcedure;
                        cmdAlerts.Parameters.AddWithValue("@BATCH_SIZE", BatchSize);

                        using var rd = await cmdAlerts.ExecuteReaderAsync(stoppingToken);
                        if (await rd.ReadAsync(stoppingToken))
                        {
                            var status = Convert.ToInt32(rd["status"]);
                            var message = Convert.ToString(rd["message"]) ?? "";

                            if (status == 1)
                                _logger.LogInformation("ALERTS: {Message}", message);
                            else
                                _logger.LogWarning("ALERTS: {Message}", message);
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR procesando ALERTS");
                    await DelayOrStopAsync(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        private static async Task DelayOrStopAsync(TimeSpan delay, CancellationToken ct)
        {
            try
            {
                await Task.Delay(delay, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
            }
        }

    }
}
