using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Notifications
{
    public class SupportAlertsWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SupportAlertsWorker> _logger;

        public SupportAlertsWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<SupportAlertsWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker SOPORTE iniciado ⏰");

            await Task.Yield();
            await DelayOrStopAsync(TimeSpan.FromSeconds(2), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var currentTime = DateTime.Now.TimeOfDay;
                    var startWindow = new TimeSpan(8, 0, 0); //(8 am)
                    var endWindow = new TimeSpan(9, 0, 0); // (9 am)

                    if (currentTime >= startWindow && currentTime <= endWindow)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var cnFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();

                        using var cn = cnFactory.CreateConnection();
                        await cn.OpenAsync(stoppingToken);

                        using (var cmd = new SqlCommand("dbo.SP_CORE_QUEUE_SUPPORT_EXPIRATION_DATE", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            await cmd.ExecuteNonQueryAsync(stoppingToken);
                        }

                        _logger.LogInformation("ALERTS: Verificación de vencimientos de Soporte ejecutada.");
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR procesando ALERTS de Soporte");
                }
                //tiempo de consulta nuevamente 
                await DelayOrStopAsync(TimeSpan.FromMinutes(5), stoppingToken);
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
