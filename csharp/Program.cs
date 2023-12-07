using Aoc;

HttpClient httpClient = new();
httpClient.DefaultRequestHeaders.Add("cookie", Environment.GetEnvironmentVariable("AOC_COOKIE"));
await new Runner(new InputHandlerFactory(httpClient))
    .RunAsync("2023", "7", args.Length > 0 ? args[0] : null);
