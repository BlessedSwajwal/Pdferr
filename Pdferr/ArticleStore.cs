using Pdferr.Model;

namespace Pdferr;

public static class ArticleStore
{
    public static List<ArticleDetails> CuratedArticles { get; set; } = [];
    public static List<ArticleDetails> TechArticles { get; set; } = [];
    public static List<ArticleDetails> EntertainmentArticles { get; set; } = [];
    public static List<ArticleDetails> ScienceArticles { get; set; } = [];
}
