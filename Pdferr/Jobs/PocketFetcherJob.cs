using Quartz;

namespace Pdferr.Jobs;

[DisallowConcurrentExecution]
public class PocketFetcherJob(PocketService pocketService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Fetching curated articles now at {DateTime.UtcNow} UTC.");
        var result = await pocketService.GetCuratedArticles(1);
        ArticleStore.CuratedArticles = result;
    }
}
