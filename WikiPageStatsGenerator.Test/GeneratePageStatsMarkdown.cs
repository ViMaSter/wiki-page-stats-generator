﻿using System.Net;
using System.Reflection;
using NUnit.Framework;

namespace WikiPageStatsGenerator.Test;

public class FileHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (Assembly.GetExecutingAssembly().GetManifestResourceStream("WikiPageStatsGenerator.Test.pages-batch.json") is not { } stream)
        {
            throw new Exception("pages-batch.json does not exist as EmbeddedResource as part of test assembly");
        }

        return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(stream)
        });
    }
}

public class GeneratePageStatsMarkdownTest
{
    [Test]
    public async Task TestExecute()
    {
        // arrange
        var client = new HttpClient(new FileHttpClientHandler());
        if (Assembly.GetExecutingAssembly().GetManifestResourceStream("WikiPageStatsGenerator.Test.expected.md") is not { } expectedStream)
        {
            throw new Exception("pages-batch.json does not exist as EmbeddedResource as part of test assembly");
        }

        var expectedStreamContent = new StreamContent(expectedStream);

        var expectedString = await expectedStreamContent.ReadAsStringAsync();

        // act
        GeneratePageStatsMarkdown.Execute(client, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
    
        // assert
        Assert.That(File.Exists(Path.Join("Most-Viewed-Pages.md")), Is.True);
        var actualString = await File.ReadAllTextAsync(Path.Join("Most-Viewed-Pages.md"));

        Assert.That(actualString, Is.EqualTo(expectedString));
    }
}