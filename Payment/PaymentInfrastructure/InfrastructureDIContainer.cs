using Flaminco.JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentEntities;
using PaymentInfrastructure.HttpClients;
using SharedModule.Middlewares;

namespace PaymentInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddTransient<JsonLocalizerManager>();
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<TPayClient>();
        services.AddDbContext<PaymentDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), a => a.MigrationsAssembly("PaymentService"));
        });
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        return services;
    }
}