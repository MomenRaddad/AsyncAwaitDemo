using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

public static class ConcurrentDemo
{
    private static readonly HttpClient _http = new HttpClient
    {
        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
    };

    public static async Task RunConcurrentAsync()
    {
        Console.WriteLine("=== CONCURRENT (Task.WhenAll) ===");

        var sw = Stopwatch.StartNew();

        var postsTask = MeasureAsync(sw,"Posts","posts");
        var usersTask = MeasureAsync(sw,"Users","users");
        var commentsTask = MeasureAsync(sw,"Comments","comments");

        var results = await Task.WhenAll(postsTask, usersTask, commentsTask);

        sw.Stop();

        foreach (var r in results)
        {
            Console.WriteLine($"{r.Name,-2}:{r.Length} chars ({r.DurationMs} ms)");
        }

        Console.WriteLine($"Total time:{sw.ElapsedMilliseconds} ms");
    }

    private static async Task<(string Name, int Length, long DurationMs)> MeasureAsync(
        Stopwatch sw, string name, string url)
    {
        var start = sw.ElapsedMilliseconds;
        var body = await GetAsync(url);
        var end = sw.ElapsedMilliseconds;

        return (name, body.Length, end - start);
    }

    private static async Task<string> GetAsync(string relativeUrl)
    {
        var response = await _http.GetAsync(relativeUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
