using System.Reflection;

namespace Aoc;

public class Runner
{
    private readonly InputHandlerFactory inputHandler;

    public Runner(InputHandlerFactory inputHandler)
    {
        this.inputHandler = inputHandler;
    }
    public async Task RunAsync(string year, string day, string? examplePath)
    {
        var result = GetSolver(year, day).Solve(
                await inputHandler.For(year, day, examplePath).GetAsync());
        Type resultType = result.GetType();
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ValueTuple<,>))
        {
            Console.WriteLine(result.Item1);
            Console.WriteLine(result.Item2);
        }
        else
        {
            Console.WriteLine(result);
        }
        
    }

    private static ISolver GetSolver(string year, string day)
    {
        return new SolverWithMeasurement((ISolver?)Assembly.GetExecutingAssembly()
            .CreateInstance($"Aoc{year}.Solver{year}{Helpers.PadDay(day)}")!);
    }

}

public interface ISolver
{
    dynamic Solve(string[] lines);
}

internal class SolverWithMeasurement : ISolver
{
    private ISolver solver;
    public SolverWithMeasurement(ISolver solver)
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