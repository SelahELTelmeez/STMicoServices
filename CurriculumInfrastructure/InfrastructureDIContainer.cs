using CurriculumInfrastructure.HttpClients;
using CurriculumInfrastructure.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace CurriculumInfrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<TrackerClient>();
        services.AddMapsterConfigration();
        return services;
    }
}
