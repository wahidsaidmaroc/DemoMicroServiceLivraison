using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceNotification.Infrastructure.Persistence;

public static class DatabaseSelector
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

        services.AddDbContext<AppDbContext>(options =>
        {
            switch (provider)
            {
                case "PostgreSql":
                    options.UseNpgsql(
                        configuration.GetConnectionString("PostgreSql"),
                        npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
                    break;

                case "MySql":
                    var mySqlConnection = configuration.GetConnectionString("MySql")!;
                    options.UseMySql(
                        mySqlConnection,
                        ServerVersion.AutoDetect(mySqlConnection),
                        mysql => mysql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
                    break;

                case "SqlServer":
                default:
                    options.UseSqlServer(
                        configuration.GetConnectionString("SqlServer"),
                        sqlServer => sqlServer.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
                    break;
            }
        });

        return services;
    }
}
