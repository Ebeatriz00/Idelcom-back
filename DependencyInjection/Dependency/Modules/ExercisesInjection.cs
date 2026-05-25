using Application.DTOs.Exercises;
using Application.UseCases.Exercises;
using Application.Validators.Exercises;
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
    public static class ExercisesInjection
    {
        public static IServiceCollection AddExercisesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateExercises>();
            services.AddScoped<GetAllExercises>();
            services.AddScoped<GetSelectExercises>();
            services.AddScoped<GetByIdExercises>();
            services.AddScoped<UpdateExercises>();
            services.AddScoped<PatchExercisesStatus>();
            services.AddScoped<ToggleBlockExercises>();

            services.AddTransient<IValidator<ExercisesCreateDto>, ExercisesCreateValidator>();
            services.AddTransient<IValidator<ExercisesUpdateDto>, ExercisesUpdateValidator>();
            services.AddTransient<IValidator<ExercisesStatusToggleDto>, ExercisesStatusToggleValidator>();
            services.AddTransient<IValidator<ExercisesBlockToggleDto>, ExercisesBlockToggleValidator>();

            services.AddScoped<IExercisesRepository, ExercisesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }

    }
}
