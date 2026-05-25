using Application.UseCases.Comment;
using Core.Interfaces;
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
    public static class CommentInjection
    {
        public static IServiceCollection AddCommentServices(this IServiceCollection services)
        {
            services.AddScoped<CreateComment>();
            services.AddScoped<GetComment>();
            services.AddScoped<MarkCommentRead>();

            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}
