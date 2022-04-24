using FluentValidation;
using IdentityEntities.Entities;
using IdentityInfrastructure.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace IdentityInfrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddMapsterConfigration();
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        services.AddValidatorsFromAssembly(typeof(IMarkupAssemblyScanning).Assembly);
        services.AddDbContext<STIdentityDbContext>(options =>
        {
            options.UseSqlServer(new SqlConnectionStringBuilder
            {
                DataSource = @"AHMED\SQLEXPRESS",
                //DataSource = @".",
                InitialCatalog = "STIdentity",
                IntegratedSecurity = true
            }.ConnectionString, a => a.MigrationsAssembly("IdentityService"));
        });
        return services;
    }
}
