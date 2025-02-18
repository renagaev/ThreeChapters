using System.ComponentModel.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Infrastructure.Implementation.BackgroundJobs;

public static class Entry
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(nameof(BackgroundJobsSettings)).Get<BackgroundJobsSettings>()!;
        services.AddQuartz(quartz =>
        {
            quartz.ScheduleJob<DailyPostJob>(trigger =>
                    trigger.WithCronSchedule(settings.DailyPostCron).WithIdentity(Constants.DailyPostTriggerKey),
                job => job.WithIdentity(Constants.DailyPostJobKey));
            quartz.UseInMemoryStore();
        });

        services.AddQuartzHostedService();

        return services;
    }
}