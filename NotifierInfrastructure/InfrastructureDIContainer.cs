namespace NotifierInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IdentityClient>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddHttpClient("FCMClient", options =>
        {
            options.BaseAddress = new Uri("https://fcm.googleapis.com/fcm");
            options.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=AAAAynh-f3A:APA91bFOziAfx9KE5HejgcvaLJn2VZGrn1HK4tUpQ9Vas7-1EObLjv6udvIW9O3COT8vLB3lROiBefarZLIv29kLUfgsmvt0j0dSHJBw5kFiy3NUv9OuSuXjP87lNkVtO184wOH9vp_9");
            options.DefaultRequestHeaders.Add("Sender", "id=869604949872");
        });
        services.AddDbContext<NotifierDbContext>(options =>
        {
            options.UseSqlServer(new SqlConnectionStringBuilder
            {
                //DataSource = @"AHMED\SQLEXPRESS",
                DataSource = @".",
                InitialCatalog = "STNotification",
                IntegratedSecurity = true
            }.ConnectionString, a => a.MigrationsAssembly("NotifierService"));
        });
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        return services;
    }
}
