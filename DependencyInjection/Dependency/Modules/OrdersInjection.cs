using Application.DTOs.Orders;
using Application.UseCases.Orders;
using Application.Validators.Orders;
using Core.Interfaces;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class OrdersInjection
    {
        public static IServiceCollection AddOrdersServices(this IServiceCollection services)
        {
            services.AddScoped<GetAllOrders>();
            services.AddScoped<RegisterSsomaOrder>();
            services.AddScoped<RegisterQualitySupervisor>();
            services.AddScoped<CreateProjectManager>();


            services.AddScoped<IValidator<OrdersSsomaRegister>, OrdersSsomaRegisterValidator>();
            services.AddScoped<IOrdersRepository, OrdersRepository> ();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}
