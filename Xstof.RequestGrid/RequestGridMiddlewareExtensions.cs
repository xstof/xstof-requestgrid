using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Xstof.RequestGrid
{
    public static class RequestGridMiddlewareExtensions
    {
        public static IServiceCollection AddRequestGrid(this IServiceCollection services, Action<RequestGridMiddlewareOptions> options){

            services.Configure(options);
            services.AddTransient<Xstof.RequestGrid.RequestGridMiddleware>();

            return services;
        }

        public static IApplicationBuilder UseRequestGrid(this IApplicationBuilder builder){
            return builder.UseMiddleware<RequestGridMiddleware>();
        }
    }
} 