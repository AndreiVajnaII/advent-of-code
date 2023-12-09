using System.Reflection;

namespace Aoc;

public class Runner(InputHandlerFactory inputHandler)
{
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

    private static SolverWithMeasurement GetSolver(string year, string day)
    {
        return new SolverWithMeasurement((ISolver?)Assembly.GetExecutingAssembly()
            .CreateInstance($"Aoc{year}.Solver{year}{Helpers.PadDay(day)}")!);
    }

}

public interface ISolver
{
    dynamic Solve(string[] lines);
}

internal class SolverWithMeasurement(ISolver solver) : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = solver.Solve(lines);
        stopwatch.Stop();
        Console.WriteLine($"Took {stopwatch.ElapsedMilliseconds:N} ms.");
        return result;
    }
}