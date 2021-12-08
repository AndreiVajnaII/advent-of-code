public class InputHandlerFactory {
    private HttpClient http;

    public InputHandlerFactory(HttpClient http) {
        this.http = http;
    }

    public IInputHandler For(string year, string day) {
        return new InputHandler(year, day, http);
    }
}

public interface IInputHandler {
    Task<string[]> GetAsync();
}

class InputHandler: IInputHandler {
    private readonly string inputPath;
    private static readonly string examplePath = "example.txt";
    private readonly string url;

    private readonly HttpClient http;

    

    public InputHandler(string year, string day, HttpClient http) {
        this.http = http;
        inputPath = Path.Combine(year, "inputs", $"{Helpers.PadDay(day)}.txt");
        url = $"https://adventofcode.com/{year}/day/{day}/input";
    }

    public async Task<string[]> GetAsync() {
        return (await (Exists(examplePath) ? ReadAsync(examplePath) : GetRealInputAsync()))
            .ReplaceLineEndings().Split(Environment.NewLine).SkipLast(1).ToArray();
    }

    private Task<string> GetRealInputAsync()
    {
        return Exists(inputPath) ? ReadAsync(inputPath) : DownloadAsync();
    }

    private bool Exists(string path) {
        return File.Exists(path);
    }

    private Task<string> ReadAsync(string path) {
        return File.ReadAllTextAsync(path);
    }

    private async Task<string> DownloadAsync() {
        var content = await http.GetStringAsync(url);
        File.WriteAllText(inputPath, content);
        return content;
    }

}