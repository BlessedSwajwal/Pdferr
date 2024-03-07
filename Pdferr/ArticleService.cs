using Pdferr.Model;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PuppeteerSharp;
using System.Text.Json;

namespace Pdferr;

public class ArticleService(HttpClient httpClient)
{
    public async Task<Article> GetArticle(string Url, CancellationToken cancellationToken)
    {
        try
        {
            var result = await httpClient.GetAsync($"/parser?url={Url}", cancellationToken);
            if (result.IsSuccessStatusCode)
            {
                var rootelement = JsonDocument.Parse(result.Content.ReadAsStream()).RootElement;
                return new Article()
                {
                    Title = rootelement.GetProperty("title").GetString()!,
                    Image = rootelement.GetProperty("lead_image_url").GetString() ?? String.Empty,
                    Content = rootelement.GetProperty("content").GetString()!
                };
            }
            else
            {
                Console.WriteLine(result.StatusCode);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return Article.Empty;
    }

    public async Task<byte[]> MasterPdfGenerator(List<string> urls, CancellationToken cancellationToken)
    {
        //Get articles
        var articles = await Task.WhenAll(urls.Select(u => GetArticle(u, cancellationToken)));

        //Get article pdfs
        List<byte[]> result = new List<byte[]>();
        foreach (var a in articles)
        {
            var res = await GetArticlePdf(a);
            result.Add(res);
        }

        //Merge the pdfs

        var mergedPdf = MergePdf(result);

        return mergedPdf;
    }

    /// <summary>
    /// Gets pdf of a single article.
    /// </summary>
    /// <param name="article">The article whose pdf is needed.</param>
    /// <returns></returns>
    public async Task<byte[]> GetArticlePdf(Article article)
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new string[] { "--no-sandbox" }
        });
        var pdfOptions = new PdfOptions()
        {
            MarginOptions = new PuppeteerSharp.Media.MarginOptions()
            {
                Bottom = "50",
                Top = "50",
                Left = "50",
                Right = "50"
            }
        };

        var page = await browser.NewPageAsync();
        var image_url = article.Image;

        await page.SetContentAsync($"<html><head><style>* {{font-size: 18px !important;}} img {{max-height: 350px; object-fit: contain;}}</style></head><body><h3>{article.Title}</h3><img src={image_url} alt=\"Image\"/> {article.Content}</body></html>");
        var pdf = await page.PdfDataAsync(pdfOptions);

        return pdf;
    }

    public byte[] MergePdf(List<byte[]> pdfs)
    {
        List<PdfDocument> lstDocuments = new List<PdfDocument>();

        // Open each PDF from the byte array and add it to the list of documents
        foreach (var pdf in pdfs)
        {
            lstDocuments.Add(PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Import));
        }

        // Create a new PDF document for the merged result
        using (PdfDocument outPdf = new PdfDocument())
        {
            foreach (var document in lstDocuments)
            {
                // Copy all pages from each document to the output document
                for (int i = 0; i < document.PageCount; i++)
                {
                    outPdf.AddPage(document.Pages[i]);
                }
            }

            // Save the merged PDF to a memory stream
            using (MemoryStream outputStream = new MemoryStream())
            {
                outPdf.Save(outputStream);
                return outputStream.ToArray();
            }
        }
    }

    public byte[] MergePdfs(byte[][] pdfs)
    {
        using (var outputStream = new MemoryStream())
        {
            using (var mergedDocument = new PdfDocument())
            {
                foreach (var pdfBytes in pdfs)
                {
                    using (var pdfStream = new MemoryStream(pdfBytes))
                    {
                        var pdfDocument = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Import);
                        for (int i = 0; i < pdfDocument.PageCount; i++)
                        {
                            mergedDocument.AddPage(pdfDocument.Pages[i]);
                        }
                    }
                }

                mergedDocument.Save(outputStream);
            }

            return outputStream.ToArray();
        }
    }

}
