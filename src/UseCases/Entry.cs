using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Services;

namespace UseCases;

public static class Entry
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddSingleton<IntervalMerger>();
        services.AddSingleton<IIntervalParser, IntervalParser>();
        services.AddSingleton<IntervalSplitter>();
        services.AddSingleton<DailyPostRenderer>();
        return services;
    }
}