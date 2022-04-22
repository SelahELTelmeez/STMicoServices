using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<CurriculumClient>();
        services.AddHttpClient<NotifierClient>();
        services.AddDbContext<TeacherDbContext>(options =>
        {
            options.UseSqlServer(new SqlConnectionStringBuilder
            {
                DataSource = @"AHMED\SQLEXPRESS",
                // DataSource = @".",
                InitialCatalog = "STTeacher",
                IntegratedSecurity = true
            }.ConnectionString);
        });
        return services;
    }
}
