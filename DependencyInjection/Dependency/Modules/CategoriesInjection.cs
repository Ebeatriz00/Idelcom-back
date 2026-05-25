using Application.DTOs.Categories;
using Application.UseCases.Categories;
using Application.Validators.Categories;
using Core.Interfaces.logistic;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Logistic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class CategoriesInjection
    {
        public static IServiceCollection AddCategoriesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateCategories>();
            services.AddScoped<GetAllCategories>();
            services.AddScoped<GetCategoriesById>();
            services.AddScoped<UpdateCategories>();
            services.AddScoped<PatchCategoriesStatus>();
            services.AddScoped<GetSelectCategories>();

            services.AddTransient<IValidator<CategoriesCreateDto>, CategoriesCreateValidator>();
            services.AddTransient<IValidator<CategoriesUpdateDto>, CategoriesUpdateValidator>();
            services.AddTransient<IValidator<CategoriesStatusToggleDto>, CategoriesStatusToggleValidator>();

            services.AddScoped<ICategoriesRepository, CategoriesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}
