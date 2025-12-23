using System;

await SequentialDemo.RunSequentialAsync();

Console.WriteLine();
Console.WriteLine(new string('-', 50));
Console.WriteLine();

await ConcurrentDemo.RunConcurrentAsync();

Console.WriteLine();
Console.WriteLine(new string('-', 50));
Console.WriteLine();

await ErrorHandlingDemo.RunAsync();

