namespace Aoc;

public class InputHandlerFactory(HttpClient http)
{
    public IInputHandler For(string year, string day, string? examplePath) {
        return new InputHandler(year, day, examplePath, http);
    }
}

public interface IInputHandler {
    Task<string[]> GetAsync();
}

class InputHandler(string year, string day, string? examplePath, HttpClient http) : IInputHandler {
    private readonly string inputPath = Path.Combine(year, "inputs", $"{Helpers.PadDay(day)}.txt");
    private readonly string url = $"https://adventofcode.com/{year}/day/{day}/input";

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