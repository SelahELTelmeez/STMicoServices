using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using TeacherDomain.Services;
using TeacherEntities.Entities;
using TeacherInfrastructure.HttpClients;
using TeacherInfrastructure.Services;
using System.Data.SqlClient;

namespace TeacherInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IdentityClient>();
        services.AddHttpClient<CurriculumClient>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddHttpClient("FCMClient", options =>
        {
            options.BaseAddress = new Uri("https://fcm.googleapis.com/fcm");
            options.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=AAAAynh-f3A:APA91bFOziAfx9KE5HejgcvaLJn2VZGrn1HK4tUpQ9Vas7-1EObLjv6udvIW9O3COT8vLB3lROiBefarZLIv29kLUfgsmvt0j0dSHJBw5kFiy3NUv9OuSuXjP87lNkVtO184wOH9vp_9");
            options.DefaultRequestHeaders.Add("Sender", "id=869604949872");
        });
        services.AddDbContext<TeacherDbContext>(options =>
        {
            options.UseSqlServer(new SqlConnectionStringBuilder
            {
                DataSource = @"AHMED\SQLEXPRESS",
                //DataSource = @".",
                InitialCatalog = "STTracker",
                IntegratedSecurity = true
            }.ConnectionString);
        });
        return services;
    }
}
