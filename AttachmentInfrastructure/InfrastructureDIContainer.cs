using AttachmentEntity;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AttachmentInfrastructure;
public static class InfrastructureDIContainer
{
    public static IServiceCollection AddInfrastructureDIContainer(this IServiceCollection services)
    {
        services.AddMediatR(typeof(IMarkupAssemblyScanning));

        services.AddDbContext<AttachmentDbContext>(options =>
        {
            options.UseSqlServer(new SqlConnectionStringBuilder
            {
                //DataSource = @"AHMED\SQLEXPRESS",
                DataSource = @".",
                InitialCatalog = "STAttachment",
                IntegratedSecurity = true
            }.ConnectionString, a => a.MigrationsAssembly("AttachmentService"));
        });
        return services;
    }
}
