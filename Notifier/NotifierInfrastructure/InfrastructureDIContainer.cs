using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotifierDomain.Services;
using NotifierEntities.Entities;
using NotifierInfrastructure.HttpClients;
using NotifierInfrastructure.Services;
using SharedModule.Middlewares;

namespace NotifierInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddHttpClient<IdentityClient>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddHttpClient("FCMClient", options =>
        {
            string authorizationKey = string.Format("key={0}", configuration["FCM:ServerKey"]);
            string senderId = string.Format("id={0}", configuration["FCM:SenderId"]);

            options.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
            options.DefaultRequestHeaders.TryAddWithoutValidation("Sender", senderId);
        });
        services.AddDbContext<NotifierDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), a => a.MigrationsAssembly("NotifierService"));
        });
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        return services;
    }
}
