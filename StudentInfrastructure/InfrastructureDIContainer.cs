using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.Middlewares;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ErrorHandlerMiddleware>();

        services.AddHttpClient<CurriculumClient>();
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        services.AddDbContext<StudentDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), a => a.MigrationsAssembly("StudentService"));
        });
        return services;
    }
}
