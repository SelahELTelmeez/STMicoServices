using CurriculumEntites.Entities;
using CurriculumInfrastructure.HttpClients;
using CurriculumInfrastructure.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.Middlewares;

namespace CurriculumInfrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<StudentClient>();
        services.AddMapsterConfigration();
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        services.AddDbContext<CurriculumDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                 a => a.MigrationsAssembly("CurriculumService"));
        });
        return services;
    }
}
