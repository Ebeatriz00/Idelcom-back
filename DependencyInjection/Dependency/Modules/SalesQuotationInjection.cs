using Application.Contracts;
using Application.DTOs.Quotation;
using Application.Services.Excel;
using Application.Services.InterfacesServices;
using Application.UseCases.Quotation;
using Application.Validators.Quotation;
using Core.Interfaces;
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
    public static class SalesQuotationInjection
    {
        public static IServiceCollection AddSalesQuotationServices(this IServiceCollection services)
        {
            services.AddScoped<CreateSalesQuotation>();
            services.AddScoped<CreateSalesQuotationVer>();
            services.AddScoped<GetAllSalesQuotation>();
            services.AddScoped<GetAllSalesQuotationVer>();
            services.AddScoped<GetDetailSalesQuotation>();

            services.AddTransient<IValidator<SalesQuotationCreateDto>, SalesQuotationCreateValidator>();

            services.AddScoped<ISalesQuotationRepository, SalesQuotationRepository>();
            services.AddScoped<ISalesQuotationExcelParserServices, SalesQuotationExcelParser>();
            services.AddScoped<IQuotationExcelValidator, QuotationExcelValidator>();
            services.AddScoped<ValidateQuotationExcel>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}
