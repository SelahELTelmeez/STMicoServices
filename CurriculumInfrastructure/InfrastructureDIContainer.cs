using CurriculumEntites.Entities;
using CurriculumInfrastructure.HttpClients;
using CurriculumInfrastructure.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace CurriculumInfrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<TrackerClient>();
        services.AddMapsterConfigration();
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        services.AddDbContext<CurriculumDbContext>(options =>
        {
            options.UseSqlServer(new SqlConnectionStringBuilder
            {
                //DataSource = @"AHMED\SQLEXPRESS",
                DataSource = @".",
                InitialCatalog = "STCurriculum",
                IntegratedSecurity = true
            }.ConnectionString);
        });
        return services;
    }
}
