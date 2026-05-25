
using Application.DTOs.PaymentType;
using Application.UseCases.PaymentType;
using Application.Validators.PaymentType;
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
    public static class PaymentTypeInjection
    {
        public static IServiceCollection AddPaymentTypeServices(this IServiceCollection services)
        {
            services.AddScoped<CreatePaymentType>();
            services.AddScoped<GetAllPaymentType>();
            services.AddScoped<GetSelectPaymentType>();
            services.AddScoped<GetByIdPaymentType>();
            services.AddScoped<UpdatePaymentType>();
            services.AddScoped<PatchPaymentType>();
            
            services.AddTransient<IValidator<PaymentTypeCreateDto>, PaymentTypeCreateValidator>();
            services.AddTransient<IValidator<PaymentTypeUpdateDto>, PaymentTypeUpdateValidator>();
            services.AddTransient<IValidator<PaymentTypeStatusToggleDto>, PaymentTypeStatusToggleValidator>();
            services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }



    }
    
}
