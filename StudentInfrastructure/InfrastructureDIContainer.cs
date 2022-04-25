using Microsoft.Extensions.DependencyInjection;
using StudentInfrastructure.HttpClients;
using StudentEntities.Entities;
using System.Data.SqlClient;

namespace StudentInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<CurriculumClient>();
        services.AddHttpClient<NotifierClient>();
        services.AddHttpClient<TeacherClient>();

        services.AddDbContext<StudentDbContext>(options =>
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
