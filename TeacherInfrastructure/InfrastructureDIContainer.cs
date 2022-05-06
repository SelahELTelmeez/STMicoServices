using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.Middlewares;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<CurriculumClient>();
        services.AddHttpClient<NotifierClient>();
        services.AddHttpClient<StudentClient>();
        services.AddDbContext<TeacherDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), a => a.MigrationsAssembly("TeacherService"));
        });
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        return services;
    }
}