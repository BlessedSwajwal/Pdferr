using Pdferr.Jobs;

namespace Pdferr;

public static class ConfigurationHandler
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddCors(options =>
        {
            options.AddPolicy("ReactFrontEnd", pb =>
            {
                pb.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
            });
        });



        services.AddSwaggerGen();
        services.AddSingleton<PocketService>();
        services.AddSingleton<ArticleService>();
        services.AddHttpClient<PocketService>(client =>
        {
            client.BaseAddress = new Uri("https://getpocket.com/v3/");
        });

        services.AddHttpClient<ArticleService>(client =>
        {
            client.BaseAddress = new Uri("http://parser:3000/");
        });

        services.AddHttpClient<TrendingArticlesFetcher>(client =>
        {
            client.BaseAddress = new Uri("https://gnews.io/api/v4/");
        });


        return services;
    }
}
