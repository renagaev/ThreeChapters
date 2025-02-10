using Infrastructure.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Implementation.DataAccess;

public static class Entry
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dataSource = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DefaultConnection"))
            .EnableDynamicJson()
            .Build();
        services.AddDbContext<IDbContext, AppDbContext>((provider, builder) =>
        {
            builder.UseNpgsql(dataSource)
                .UseSnakeCaseNamingConvention();
        });

        return services;
    }
}