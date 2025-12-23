using System;
using System.Net.Http;
using System.Threading.Tasks;

public static class ErrorHandlingDemo
{
    private static readonly HttpClient _http = new HttpClient
    {
        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
    };

    public static async Task RunAsync()
    {
        Console.WriteLine("=== PHASE 3: ERROR HANDLING ===");

        await SequentialErrorAsync();
        Console.WriteLine(new string('-', 50));
        await WhenAllErrorAsync();
    }

    private static async Task SequentialErrorAsync()
    {
        Console.WriteLine("-> Sequential errors (await one-by-one)");

        try
        {
            var ok1 = await GetAsync("posts");
            Console.WriteLine($"posts OK ({ok1.Length} chars)");

            var fail = await GetAsync("this-endpoint-does-not-exist");
            Console.WriteLine($"fail OK ({fail.Length} chars)"); 
            var ok2 = await GetAsync("users");
            Console.WriteLine($"users OK ({ok2.Length} chars)");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Caught exception (sequential):");
            Console.WriteLine(ex.GetType().Name + ": " + ex.Message);
        }
    }

        private static async Task WhenAllErrorAsync()
    {
        Console.WriteLine("-> WhenAll errors (concurrent)");

        var t1 = GetAsync("posts");
        var t2 = GetAsync("bad-url-1"); 
        var t3 = GetAsync("bad-url-2"); 
        try
        {
            Console.WriteLine($" i am in try  | thread={Environment.CurrentManagedThreadId}");

            var results = await Task.WhenAll(t1, t2, t3);
            Console.WriteLine("All succeeded (unexpected). Count = " + results.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Caught exception (WhenAll):");
            Console.WriteLine(ex.GetType().Name + ": " + ex.Message);

            PrintTask("t1(posts)", t1);
            PrintTask("t2(bad-url-1)", t2);
            PrintTask("t3(bad-url-2)", t3);
        }
    }

    private static void PrintTask(string name, Task<string> t)
    {
        Console.WriteLine($"  {name}: Status={t.Status}");

        if (t.IsFaulted && t.Exception is not null)
        {
                        Console.WriteLine($"    Exceptions count: {t.Exception.InnerExceptions.Count}");
            foreach (var inner in t.Exception.InnerExceptions)
                Console.WriteLine($"    - {inner.GetType().Name}: {inner.Message}");
        }
    }

    private static async Task<string> GetAsync(string relativeUrl)
    {
        Console.WriteLine($"START {relativeUrl} | thread={Environment.CurrentManagedThreadId}");

        var response = await _http.GetAsync(relativeUrl);
        Console.WriteLine($"HEADERS {relativeUrl} status={(int)response.StatusCode}"); 
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"DONE {relativeUrl}");

        return body;
    }
}
