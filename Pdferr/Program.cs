using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pdferr;
using Pdferr.Jobs;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.RegisterServices();

    var pocketSettings = new PocketSettings();
    builder.Configuration.GetSection(PocketSettings.SectionName).Bind(pocketSettings);
    builder.Services.AddSingleton(Options.Create<PocketSettings>(pocketSettings));

    builder.Services.ConfigureJobs();
    builder.Services.AddQuartzHostedService();
}



var app = builder.Build();
app.UseCors("ReactFrontEnd");

app.MapGet("/articles", ([FromQuery(Name = "offset")] int offset, CancellationToken token) =>
{
    //var articles = await articleService.GetCuratedArticles(offset);
    return Results.Ok(ArticleStore.CuratedArticles);
});

app.MapPost("/articles", async (ArticleService articleService, List<string> urls, CancellationToken token) =>
{
    Console.WriteLine("Here");
    var res = await articleService.MasterPdfGenerator(urls, token);
    return Results.File(res, contentType: "application/pdf", fileDownloadName: "pdferr.pdf");
});

app.MapGet("/articles/fetch", ([FromQuery(Name = "genre")] string genre) =>
{
    var list = genre switch
    {
        "tech" => ArticleStore.TechArticles,
        "entertainment" => ArticleStore.EntertainmentArticles,
        "science" => ArticleStore.ScienceArticles,
        _ => ArticleStore.ScienceArticles,
    };

    return Results.Ok(list);
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
