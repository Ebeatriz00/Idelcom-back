using Application.DTOs.PaymentMethod;
using Application.UseCases.PaymentMethod;
using Application.Validators.PaymentMethod;
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
    public static class PaymentMethodInjection
    {
        public static IServiceCollection AddPaymentMethodServices(this IServiceCollection services)
        {
            services.AddScoped<CreatePaymentMethod>();
            services.AddScoped<GetAllPaymentMethods>();
            services.AddScoped<GetPaymentMethodById>();
            services.AddScoped<UpdatePaymentMethod>();
            services.AddScoped<PatchPaymentMethodStatus>();
            services.AddScoped<GetSelectPaymentMethod>();

            services.AddTransient<IValidator<PaymentMethodCreateDto>, PaymentMethodCreateValidator>();
            services.AddTransient<IValidator<PaymentMethodUpdateDto>, PaymentMethodUpdateValidator>();
            services.AddTransient<IValidator<PaymentMethodStatusToggleDto>, PaymentMethodStatusToggleValidator>();

            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}
