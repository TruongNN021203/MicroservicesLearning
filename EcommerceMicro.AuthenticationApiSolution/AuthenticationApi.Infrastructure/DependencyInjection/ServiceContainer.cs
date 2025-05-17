using AuthenticationApi.Application.Interface;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using Ecommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        //add database connectivity

        //JWT add authentication scheme

        SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(
            services,
            config,
            config["MySerilog:FileName"]!
        );

        //create dependency injection
        services.AddScoped<IUser, UserRepository>();
        return services;
    }

    public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
    {
        //register middleware such as:
        //global exception: handle external errors
        //listen only to api gateway

        SharedServiceContainer.UseSharesPolicies(app);
        return app;
    }
}
