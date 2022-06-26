using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.Middlewares;
using StudentDomain.Services;
using StudentInfrastructure.HttpClients;
using StudentInfrastructure.Services;
using System.Net.Http.Headers;

namespace StudentInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ErrorHandlerMiddleware>();

        services.AddHttpClient<CurriculumClient>();
        services.AddHttpClient<IdentityClient>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddHttpClient("FCMClient", options =>
        {
            options.BaseAddress = new Uri("https://fcm.googleapis.com/fcm");
            options.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=AAAAynh-f3A:APA91bFOziAfx9KE5HejgcvaLJn2VZGrn1HK4tUpQ9Vas7-1EObLjv6udvIW9O3COT8vLB3lROiBefarZLIv29kLUfgsmvt0j0dSHJBw5kFiy3NUv9OuSuXjP87lNkVtO184wOH9vp_9");
            options.DefaultRequestHeaders.Add("Sender", "id=869604949872");
        });
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        services.AddDbContext<StudentDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), a => a.MigrationsAssembly("StudentService"));
        });
        return services;
    }
}
