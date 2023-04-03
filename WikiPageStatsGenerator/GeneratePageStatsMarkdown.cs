using System.Net.Http.Headers;
using System.Text;
using Cocona;
using Newtonsoft.Json;

namespace WikiPageStatsGenerator;

public abstract class GeneratePageStatsMarkdown
{
    // ReSharper disable InconsistentNaming - we want to use the same names as the JSON response from Azure DevOps
    // ReSharper disable ClassNeverInstantiated.Local - false positive
    private sealed record WikiPageDetail(
        Value[] value
    );

    private sealed record Value(
        string path,
        ViewStats[] viewStats
    );

    private sealed record ViewStats(
        string day,
        int count
    );
    // ReSharper restore ClassNeverInstantiated.Local
    // ReSharper restore InconsistentNaming
    
    private const string AUTO_GENERATION_INFO = "<!-- !!! THIS FILE IS AUTOGENERATED - DO NOT EDIT IT MANUALLY !!! -->";
    private static readonly string AutoGenerationHeader = $"{AUTO_GENERATION_INFO}{Environment.NewLine}{AUTO_GENERATION_INFO}{Environment.NewLine}{AUTO_GENERATION_INFO}{Environment.NewLine}{Environment.NewLine}";

    public static void Execute([FromService]HttpClient httpClient, [FromService]DateTime now, string azurePAT, string organization, string project, string wikiIdentifier, string outputPath)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($":{azurePAT}")));
        
        var responseBody = httpClient.PostAsync(
            $"https://dev.azure.com/{organization}/{project}/_apis/wiki/wikis/{wikiIdentifier}/pagesbatch?api-version=7.0",
            new StringContent(
                JsonConvert.SerializeObject(new
                {
                    pageViewsForDays = 30,
                    top = 100,
                }),
                Encoding.UTF8,
                "application/json"
            )
        ).Result.Content.ReadAsStringAsync().Result;
        
        var wikiPageDetail = JsonConvert.DeserializeObject<WikiPageDetail>(responseBody)!;
        var pageViewCountByDay = new SortedDictionary<string, Dictionary<string, int>>();
        foreach (var value in wikiPageDetail.value)
        {
            foreach (var viewStats in value.viewStats)
            {
                var humanReadableDay = DateTime.Parse(viewStats.day).ToString("dd.MM.yyyy");
                var pageName = value.path[1..];
                if (!pageViewCountByDay.ContainsKey(pageName))
                {
                    pageViewCountByDay[pageName] = new Dictionary<string, int>();
                }
                pageViewCountByDay[pageName][humanReadableDay] = viewStats.count;
            }
        }
        
        var orderedKeysByTotalViews = pageViewCountByDay.Keys.OrderByDescending(key => pageViewCountByDay[key].Values.Sum()).ToArray();
        
        var last30Days = Enumerable.Range(1, 30).Select(i => now.AddDays(-i).ToString("dd.MM.yyyy")).ToArray();

        var markdownTable = new StringBuilder();
        markdownTable.AppendLine(AutoGenerationHeader);
        markdownTable.AppendLine("| Page | Total | " + string.Join(" | ", last30Days) + " |");
        markdownTable.AppendLine("| --- | --- | " + string.Join(" | ", last30Days.Select(_ => "---")) + " |");
        foreach (var page in orderedKeysByTotalViews)
        {
            var viewCountByDay = pageViewCountByDay[page];
            markdownTable.AppendLine($"| {page.Replace("|", "\\|")} | {viewCountByDay.Values.Sum()} | " + string.Join(" | ", last30Days.Select(day => viewCountByDay.TryGetValue(day, out var value) ? value.ToString() : "0")) + " |");
        }
        
        File.WriteAllText(Path.Join(outputPath, "Most-Viewed-Pages.md"), markdownTable.ToString());
    }
}