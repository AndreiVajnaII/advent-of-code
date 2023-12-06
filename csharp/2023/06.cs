using Aoc;

namespace Aoc2023;

public class Solver202306 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var times = ParseNumbers(lines[0]);
        var distances = ParseNumbers(lines[1]);

        return (
            times.Zip(distances)
                .Select(WinningPossibilities)
                .Product(),
            WinningPossibilities((
                AdjustedNumber(lines[0]),
                AdjustedNumber(lines[1])
            ))
        );
    }

    private static long AdjustedNumber(string line) =>
        long.Parse(line.Split(":")[1].Replace(" ", ""));

    private static long WinningPossibilities((long Time, long Distance) race)
    {
        var delta = Math.Sqrt(race.Time * race.Time - 4 * race.Distance);
        var roundedDelta = Math.Round(delta);
        var lowerBound = (long)Math.Ceiling((race.Time - delta) / 2);
        var upperBound = (long)Math.Floor((race.Time + delta) / 2);
        return delta == roundedDelta ? upperBound - lowerBound - 1 : upperBound - lowerBound + 1;
    }

    private static long[] ParseNumbers(string line) =>
        line.Split(":")[1]
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();
}