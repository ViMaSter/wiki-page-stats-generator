using System.Diagnostics.CodeAnalysis;
using Cocona;
using Cocona.Lite;
using Polly;

namespace WikiPageStatsGenerator;

[ExcludeFromCodeCoverage]
internal abstract class Program
{
    public static async Task Main()
    {
        var builder = CoconaLiteApp.CreateBuilder();
        builder.Services.AddSingleton(
            Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(15),
                })
                .ExecuteAsync(() => Task.FromResult(new HttpClient())).Result
        );
        builder.Services.AddSingleton(DateTime.Now);
        
        var app = builder.Build();
        app.AddCommand(GeneratePageStatsMarkdown.Execute);
        await app.RunAsync();
    }
}