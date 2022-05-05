using Microsoft.Extensions.DependencyInjection;
using ParentInfrastructure.HttpClients;
using SharedModule.Middlewares;

namespace ParentInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<TeacherClient>();
        services.AddHttpClient<NotifierClient>();

        return services;
    }
}