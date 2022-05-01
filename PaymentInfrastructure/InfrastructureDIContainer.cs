using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentEntities;

namespace PaymentInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddDbContext<PaymentDbContext>(options =>
        {
            options.UseSqlServer(new SqlConnectionStringBuilder
            {
                DataSource = @"AHMED\SQLEXPRESS",
                //DataSource = @".",
                InitialCatalog = "STPayment",
                IntegratedSecurity = true
            }.ConnectionString, a => a.MigrationsAssembly("PaymentService"));
        });
        services.AddMediatR(typeof(IMarkupAssemblyScanning));
        return services;
    }
}