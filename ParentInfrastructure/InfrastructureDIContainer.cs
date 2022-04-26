using Microsoft.Extensions.DependencyInjection;
using ParentInfrastructure.HttpClients;

namespace ParentInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<TeacherClient>();
        services.AddHttpClient<NotifierClient>();

        return services;
    }
}