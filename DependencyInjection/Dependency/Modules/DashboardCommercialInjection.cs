using Application.UseCases.Dashboard;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class DashboardCommercialInjection
    {
        public static IServiceCollection AddDashboardServices(this IServiceCollection services)
        {
            services.AddScoped<IGetDashboardMetricsUseCase, GetDashboardMetricsUseCase>();
            services.AddScoped<IGetDashboardMetricsClientsUseCase, GetDashboardMetricsClientsUseCase>();
            services.AddScoped<IGetDashboardMetricsQuarterUseCase, GetDashboardMetricsQuarterUseCase>();
            services.AddScoped<IGetDashboardMetricsCombinedUseCase,  GetDashboardMetricsCombinedUseCase>();
            services.AddScoped<IGetDashboardProbabilityUseCase, GetDashboardProbabilityAmountUseCase>();
            services.AddScoped<IGetDashboardEvolutionUseCase, GetDashboardCommercialEvolutionUseCase>();
            services.AddScoped<IGetDashboardClosingUseCase, GetDashboardClosingUseCase>();
            services.AddScoped<IGetDashboardClientOpportunityUseCase, GetDashboardClientOpportunityUseCase>();
            services.AddScoped<IGetDashboardTotalsUseCase, GetDashboardCommercialTotals>();
            services.AddScoped<IDashboardCommercialRepository, DashboardCommercialRepository>();

            return services;
        }
    }
}
