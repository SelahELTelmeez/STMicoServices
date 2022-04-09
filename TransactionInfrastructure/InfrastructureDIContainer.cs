using Microsoft.Extensions.DependencyInjection;
using TransactionInfrastructure.HttpClients;

namespace TransactionInfrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<CurriculumClient>();
        return services;
    }
}
