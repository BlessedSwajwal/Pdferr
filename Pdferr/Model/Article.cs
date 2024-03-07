namespace Pdferr.Model;

public class Article
{
    public string Title { get; set; }
    public string Image { get; set; }
    public string Content { get; set; }

    public static readonly Article Empty = new();

}
