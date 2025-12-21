using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

public static class SequentialDemo
{
    private static readonly HttpClient _http = new HttpClient
    {
        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
    };

    public static async Task RunSequentialAsync()
    {
        Console.WriteLine("=== SEQUENTIAL (await one-by-one) ===");

        var sw = Stopwatch.StartNew();

        var posts = await MeasureAsync(sw, "Posts", "posts");
        Console.WriteLine($"Posts:{posts.Length} chars ({posts.DurationMs} ms)");

        var users = await MeasureAsync(sw, "Users", "users");
        Console.WriteLine($"Users:{users.Length} chars ({users.DurationMs} ms)");

        var comments = await MeasureAsync(sw, "Comments", "comments");
        Console.WriteLine($"Comments:{comments.Length} chars ({comments.DurationMs} ms)");

        sw.Stop();
        Console.WriteLine($"Total time: {sw.ElapsedMilliseconds} ms");
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
