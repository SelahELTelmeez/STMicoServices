using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using TransactionDomain.Services;
using TransactionEntites.Entities;
using TransactionInfrastructure.HttpClients;
using TransactionInfrastructure.Services;

namespace TransactionInfrastructure;

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
