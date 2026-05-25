using Application.DTOs.Operations.OperationsWorkOrderResponsible;
using Application.UseCases.Operations.OperationsWorkOrderResponsible;
using Application.Validators.Operations.OperationsWorkOrderResponsible;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class OperationsWorkOrderResponsibleInjection
    {
        public static IServiceCollection AddOperationsWorkOrderResponsibleInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateOperationsWorkOrderResponsible>();
            services.AddScoped<UpdateOperationsWorkOrderResponsible>();
            services.AddScoped<GetByIdOperationsWorkOrderResponsible>();
            services.AddScoped<GetAllOperationsWorkOrderResponsible>();
            services.AddScoped<DeleteOperationsWorkOrderResponsible>();

            services.AddTransient<IValidator<OperationsWorkOrderResponsibleCreateDto>, OperationsWorkOrderResponsibleCreateValidator>();
            services.AddTransient<IValidator<OperationsWorkOrderResponsibleUpdateDto>, OperationsWorkOrderResponsibleUpdateValidator>();

            services.AddScoped<IOperationsWorkOrderResponsibleRepository, OperationsWorkOrderResponsibleRepository>();

            return services;
        }
    }

}
