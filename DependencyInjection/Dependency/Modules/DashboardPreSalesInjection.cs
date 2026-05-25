using Application.UseCases.DashboardPreSales;
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
    public static class DashboardPreSalesInjection
    {
        public static IServiceCollection AddDashboardPreSalesServices(this IServiceCollection services)
        {
            services.AddScoped<IGetDashboardPreSalesTotalQuotation, GetDashboardPreSalesTotalQuotation>();
            services.AddScoped<IGetDashboardPreSalesState, GetDashboardPreSalesState>();
            services.AddScoped<IGetDashboardPreSalesCombined, GetDashboardPreSalesCombined>();
            services.AddScoped<IGetDashboardPreSalesByEngineer, GetDashboardPreSalesByEngineer>();
            services.AddScoped<IGetDashboardPreSalesMatriz, GetDashboardPreSalesMatriz>();
            services.AddScoped<IDashboardPreSalesRepository, DashboardPreSalesRepository>();
            services.AddScoped<IGetDashboardPreSalesCollaborator, GetDashboardPreSalesCollaborator>();
            services.AddScoped<IGetDashboardPreSalesIntegrators, GetDashboardPreSalesIntegrators>();
            services.AddScoped<IGetDashboardPreSalesByEngineerDetails, GetDashboardPreSalesByEngineerDetails>();
            services.AddScoped<IGetDashboardPreSalesByIntegratorsDetails, GetDashboardPreSalesByIntegratorsDetails>();
            services.AddScoped<IGetDashboardPreSalesByCollaboratorDetails, GetDashboardPreSalesByCollaboratorDetails>();
            services.AddScoped<IGetDashboardPreSalesByCategory, GetDashboardPreSalesByCategory>();
            return services;
        }
    }
}
