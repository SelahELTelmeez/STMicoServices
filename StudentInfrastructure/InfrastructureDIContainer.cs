using Microsoft.Extensions.DependencyInjection;
using StudentInfrastructure.HttpClients;
using System.Data.SqlClient;

namespace StudentInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<CurriculumClient>();
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        services.AddDbContext<StudentDbContext>(options =>
        {
            options.UseSqlServer(new SqlConnectionStringBuilder
            {
                //DataSource = @"AHMED\SQLEXPRESS",
                DataSource = @".",
                InitialCatalog = "STStudent",
                IntegratedSecurity = true
            }.ConnectionString, a => a.MigrationsAssembly("StudentService"));
        });
        return services;
    }
}
