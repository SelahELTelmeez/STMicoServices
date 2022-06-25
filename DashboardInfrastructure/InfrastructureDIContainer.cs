using DashboardEntity.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.Middlewares;

namespace DashboardInfrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        services.AddDbContext<DashboardDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                 a => a.MigrationsAssembly("DashboardService"));


        });
        return services;
    }
}
