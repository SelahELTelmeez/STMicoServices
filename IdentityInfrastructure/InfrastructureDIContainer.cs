using FluentValidation;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityInfrastructure.HttpClients;
using IdentityInfrastructure.Mapping;
using IdentityInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.Middlewares;

namespace IdentityInfrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddHttpClient<NotifierClient>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddMapsterConfigration();
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        services.AddValidatorsFromAssembly(typeof(IMarkupAssemblyScanning).Assembly);
        services.AddDbContext<STIdentityDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), a => a.MigrationsAssembly("IdentityService"));
        });
        return services;
    }
}
