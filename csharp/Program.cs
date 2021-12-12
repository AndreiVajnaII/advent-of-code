HttpClient httpClient = new();
httpClient.DefaultRequestHeaders.Add("cookie", args[0]);
await new Runner(new InputHandlerFactory(httpClient))
    .RunAsync("2021", "12", args.Length > 1 ? args[1] : null);