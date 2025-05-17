using Ecommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interface;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            //add database Connectivity
            //add authentication scheme
            SharedServiceContainer.AddSharedServices<OrderDbContext>(services, config, config["MySerilog:FileName"]!);

            //create dependency injection
            services.AddScoped<IOrder, OrderRepository>();

            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            //register middleware such as
            //global exception ->handle externak errors
            //listenToApigateway only -> clock all oursiders calls 
            SharedServiceContainer.UseSharesPolicies(app);

            return app;
        }
    }
}
