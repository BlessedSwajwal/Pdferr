using Microsoft.Extensions.Options;
using Pdferr.Model;
using System.Text;
using System.Text.Json;

namespace Pdferr;

public class PocketService(HttpClient httpClient, IOptions<PocketSettings> pocketAPISettings)
{

    public async Task<List<ArticleDetails>> GetCuratedArticles(int offset)
    {
        var articleDetails = new List<ArticleDetails>();
        try
        {
            Object postObject = new
            {
                consumer_key = $"{Environment.GetEnvironmentVariable("CONSUMER_KEY")}",
                access_token = $"{Environment.GetEnvironmentVariable("ACCESS_TOKEN")}",
                count = 20,
                offset,
                contentType = "article",
                sort = "newest"
            };
            var postJson = JsonSerializer.Serialize(postObject);
            var postContent = new StringContent(postJson, encoding: Encoding.UTF8, "application/json");
            Console.WriteLine(httpClient.BaseAddress);

            var result = await httpClient.PostAsync("get", postContent);

            if (result.IsSuccessStatusCode)
            {
                var root = JsonDocument.Parse(result.Content.ReadAsStream()).RootElement;
                var list = root.GetProperty("list");
                foreach (var element in list.EnumerateObject())
                {
                    var rootElement = JsonDocument.Parse(element.Value.ToString()).RootElement;
                    var url = rootElement.GetProperty("given_url").GetString();
                    var title = rootElement.GetProperty("resolved_title").GetString();
                    JsonElement imageElement = new();
                    bool has_image_url = rootElement.TryGetProperty("top_image_url", out imageElement);
                    var excerpt = rootElement.GetProperty("excerpt").GetString();

                    string? imageUrl = has_image_url ? imageElement.GetString() : string.Empty;

                    articleDetails.Add(new ArticleDetails(url, title, imageUrl, excerpt));
                }
            }
            else
            {
                Console.WriteLine(result.StatusCode);
                Console.WriteLine(result);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return articleDetails;
    }

}
