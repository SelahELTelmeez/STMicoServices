using CurriculumEntites.Entities;
using CurriculumInfrastructure.HttpClients;
using CurriculumInfrastructure.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.Middlewares;

namespace CurriculumInfrastructure;

public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<StudentClient>();
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
            }.ConnectionString, a => a.MigrationsAssembly("CurriculumService"));
        });
        return services;
    }
}
