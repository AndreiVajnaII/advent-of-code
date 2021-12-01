using System.Reflection;

public class Runner {
    private InputHandlerFactory inputHandler;

    public Runner(InputHandlerFactory inputHandler) {
        this.inputHandler = inputHandler;
    }
    public async Task RunAsync(string year, string day) {
        Console.WriteLine(
            GetSolver(year, day)?
                .Solve(
                    await inputHandler.For(year, day).GetAsync()
                ));
    }

    private static ISolver? GetSolver(string year, string day) {
        return (ISolver?)Assembly.GetExecutingAssembly().CreateInstance("Solver" + year + Helpers.PadDay(day));
    }

}

public interface ISolver {
    dynamic Solve(string[] lines);
}