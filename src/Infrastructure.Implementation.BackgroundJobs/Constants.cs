using Quartz;

namespace Infrastructure.Implementation.BackgroundJobs;

internal static class Constants
{
    public static readonly TriggerKey DailyPostTriggerKey = new("DailyPostTrigger");
    public static readonly JobKey DailyPostJobKey = new("DailyPostJob");
}