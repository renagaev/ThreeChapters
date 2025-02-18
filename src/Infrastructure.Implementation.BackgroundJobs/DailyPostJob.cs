using MediatR;
using Quartz;
using UseCases.AddDailyPost;

namespace Infrastructure.Implementation.BackgroundJobs;

internal class DailyPostJob(ISender sender) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await sender.Send(new AddDailyPostCommand(), context.CancellationToken);
    }
}