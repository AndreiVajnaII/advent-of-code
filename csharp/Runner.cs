using System.Reflection;

public class Runner
{
    private InputHandlerFactory inputHandler;

    public Runner(InputHandlerFactory inputHandler)
    {
        this.inputHandler = inputHandler;
    }
    public async Task RunAsync(string year, string day, string? examplePath)
    {
        Console.WriteLine(
            GetSolver(year, day)
                .Solve(
                    await inputHandler.For(year, day, examplePath).GetAsync()
                ));
    }

    private static ISolver GetSolver(string year, string day)
    {
        return new SolverWithMeasureMent((ISolver?)Assembly.GetExecutingAssembly()
            .CreateInstance("Solver" + year + Helpers.PadDay(day))!);
    }

}

public interface ISolver
{
    dynamic Solve(string[] lines);
}

class SolverWithMeasureMent : ISolver
{
    private ISolver solver;
    public SolverWithMeasureMent(ISolver solver)
    {
        this.solver = solver;
    }
    public dynamic Solve(string[] lines)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = solver.Solve(lines);
        stopwatch.Stop();
        Console.WriteLine($"Took {stopwatch.ElapsedMilliseconds:N} ms.");
        return result;
    }
}