using Aoc;

namespace Aoc2023;

public class Solver202309 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var report = lines.Select(line => line.Split(" ").Select(long.Parse).ToArray()).ToArray();
        return (
            report
                .Select(Extrapolate)
                .Sum(),
            report
                .Select(Enumerable.Reverse).Select(Enumerable.ToArray)
                .Select(Extrapolate)
                .Sum()
        );
    }

    private static long Extrapolate(long[] sequence)
    {
        return sequence.All(value => value == 0) ? 0 : sequence[^1] + Extrapolate(Differences(sequence));
    }

    private static long[] Differences(long[] sequence)
    {
        return sequence.ZipShift().Select(pair => pair.Item2 - pair.Item1).ToArray();
    }
}