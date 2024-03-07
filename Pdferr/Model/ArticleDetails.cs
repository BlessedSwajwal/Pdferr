namespace Pdferr.Model;

public class ArticleDetails
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string? Image { get; set; }
    public string Excerpt { get; set; }

    public ArticleDetails(string url, string title, string? image, string excerpt)
    {
        Url = url;
        Title = title;
        Image = image;
        Excerpt = excerpt;
    }
}
