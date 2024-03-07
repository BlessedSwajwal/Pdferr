using Quartz;

namespace Pdferr.Jobs;

public static class QuartzConfigure
{
    public static IServiceCollection ConfigureJobs(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            options.AddJob<PocketFetcherJob>(JobKey.Create(nameof(PocketFetcherJob)))
                .AddTrigger(trigger =>
                {
                    trigger.ForJob(JobKey.Create(nameof(PocketFetcherJob)))
                        .StartNow()
                        .WithSimpleSchedule(schedule =>
                        {
                            schedule.WithIntervalInHours(4)
                                .RepeatForever();
                        });
                });
        });

        services.AddQuartz(options =>
        {
            options.AddJob<TrendingArticlesFetcher>(JobKey.Create(nameof(TrendingArticlesFetcher)))
                .AddTrigger(trigger =>
                {
                    trigger.ForJob(JobKey.Create(nameof(TrendingArticlesFetcher)))
                        .StartNow()
                        .WithSimpleSchedule(schedule =>
                        {
                            schedule.WithIntervalInHours(3)
                                .RepeatForever();
                        });
                });
        });

        return services;
    }
}
