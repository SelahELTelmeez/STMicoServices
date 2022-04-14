using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TransactionEntites.Entities;
using TransactionInfrastructure.HttpClients;

namespace TransactionInfrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<CurriculumClient>();
        services.AddDbContext<TrackerDbContext>(options =>
        {
            options.UseSqlServer(new SqlConnectionStringBuilder
            {
                //DataSource = @"AHMED\SQLEXPRESS",
                DataSource = @".",
                InitialCatalog = "STTracker",
                IntegratedSecurity = true
            }.ConnectionString);
        });
        return services;
    }
}
