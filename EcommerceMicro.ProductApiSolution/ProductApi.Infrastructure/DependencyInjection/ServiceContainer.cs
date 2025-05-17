using Ecommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            //add database connect
            //add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FileName"]);

            //Create dependency Injection DI
            services.AddScoped<IProduct, ProductRepository>();

            return services;

        }
        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {

            //register middleware such as:
            //global exception: handles external errors
            //listen to only api gateway: blocks all outsider calls
            SharedServiceContainer.UseSharesPolicies(app);



            return app;

        }


    }
}
