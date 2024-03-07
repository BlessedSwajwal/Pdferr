using Pdferr.Model;
using Quartz;
using System.Text.Json;

namespace Pdferr.Jobs;

public class TrendingArticlesFetcher(HttpClient client) : IJob
{
    //TODO: Secure the key
    public string apiKey = Environment.GetEnvironmentVariable("GNEWS_KEY");
    public async Task Execute(IJobExecutionContext context)
    {
        var techArticles = ArticlesFetch("technology");
        var entertainmentArticles = ArticlesFetch("entertainment");
        var scienceArticles = ArticlesFetch("science");

        ArticleStore.TechArticles = await techArticles;
        ArticleStore.EntertainmentArticles = await entertainmentArticles;
        ArticleStore.ScienceArticles = await scienceArticles;
    }

    /// <summary>
    /// Fetch articles from gnews based on genre. Should be one of: technology, entertainment and science
    /// </summary>
    /// <param name="genre"></param>
    /// <returns></returns>
    public async Task<List<ArticleDetails>> ArticlesFetch(string genre)
    {
        List<ArticleDetails> articles = new List<ArticleDetails>();
        try
        {
            var result = await client.GetAsync($"top-headlines?category={genre}&apikey={apiKey}&lang=en");
            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine(result);
            }
            var articleList = JsonDocument.Parse(result.Content.ReadAsStream()).RootElement.GetProperty("articles");

            foreach (var articleJsonElement in articleList.EnumerateArray())
            {
                var article = JsonDocument.Parse(articleJsonElement.ToString()).RootElement;
                var url = article.GetProperty("url").GetString();
                var title = article.GetProperty("title").GetString();
                var excerpt = article.GetProperty("content").GetString();
                var image = article.GetProperty("image").GetString();

                articles.Add(new ArticleDetails(url, title, image, excerpt));
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return articles;
    }
}
