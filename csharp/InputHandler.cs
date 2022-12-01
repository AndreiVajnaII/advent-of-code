namespace Aoc;

public class InputHandlerFactory {
    private HttpClient http;

    public InputHandlerFactory(HttpClient http) {
        this.http = http;
    }

    public IInputHandler For(string year, string day, string? examplePath) {
        return new InputHandler(year, day, examplePath, http);
    }
}

public interface IInputHandler {
    Task<string[]> GetAsync();
}

class InputHandler: IInputHandler {
    private readonly string inputPath;
    private readonly string? examplePath;
    private readonly string url;

    private readonly HttpClient http;

    

    public InputHandler(string year, string day, string? examplePath, HttpClient http) {
        this.http = http;
        this.examplePath = examplePath;
        inputPath = Path.Combine(year, "inputs", $"{Helpers.PadDay(day)}.txt");
        url = $"https://adventofcode.com/{year}/day/{day}/input";
    }

    public async Task<string[]> GetAsync() {
        return (await (examplePath is not null ? ReadAsync(examplePath) : GetRealInputAsync()))
            .ReplaceLineEndings().Split(Environment.NewLine).SkipLast(1).ToArray();
    }

    private Task<string> GetRealInputAsync()
    {
        return Exists(inputPath) ? ReadAsync(inputPath) : DownloadAsync();
    }

    private async Task<string> DownloadAsync() {
        var content = await http.GetStringAsync(url);
        File.WriteAllText(inputPath, content);
        return content;
    }

    private static bool Exists(string path) {
        return File.Exists(path);
    }

    private static Task<string> ReadAsync(string path) {
        return File.ReadAllTextAsync(path);
    }

}