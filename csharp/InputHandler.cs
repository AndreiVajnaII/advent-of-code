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
    private readonly string path;
    private readonly string url;

    private readonly HttpClient http;

    

    public InputHandler(string year, string day, HttpClient http) {
        this.http = http;
        path = Path.Combine(year, "inputs", $"{Helpers.PadDay(day)}.txt");
        url = $"https://adventofcode.com/{year}/day/{day}/input";
    }

    public async Task<string[]> GetAsync() {
        return (await (Exists() ? ReadAsync() : DownloadAsync()))
            .Split('\n').SkipLast(1).ToArray();
    }

    private bool Exists() {
        return File.Exists(path);
    }

    private Task<string> ReadAsync() {
        return File.ReadAllTextAsync(path);
    }

    private async Task<string> DownloadAsync() {
        var content = await http.GetStringAsync(url);
        File.WriteAllText(path, content);
        return content;
    }

}