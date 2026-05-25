using Application.UseCases.Operations.WorkDayStatus;
using Core.Interfaces.Operations;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class WorkDayStatusInjection
    {
        public static IServiceCollection AddWorkDayStatusInjection(this IServiceCollection services)
        {
            services.AddScoped<GetSelectWorkDayStatus>();
            services.AddScoped<GetByIdWorkDayStatus>();
            services.AddScoped<IWorkDayStatusRepository, WorkDayStatusRepository>();
            return services;
        }
    }
}
