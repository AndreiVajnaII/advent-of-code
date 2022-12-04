using Aoc;

namespace Aoc2022;

public class Solver202204 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var pairs = lines.Select(ParsePair).ToArray();
        return (
            pairs.Where(IsFullyContainedByOne).Count(),
            pairs.Where(IsOverlapping).Count()
        );
    }

    private static ((long, long), (long, long)) ParsePair(string line)
    {
        return line.Split(",").Select(ParseSequence).AsTuple2();
    }

    private static (long, long) ParseSequence(string sequence)
    {
        return sequence.Split("-").Select(long.Parse).AsTuple2();
    }

    private static bool IsFullyContainedByOne(((long, long), (long, long)) pair)
    {
        return FullyContains(pair.Item1, pair.Item2)
               || FullyContains(pair.Item2, pair.Item1);
    }

    private static bool IsOverlapping(((long Start, long End), (long Start, long End)) pair)
    {
        return IsInSequence(pair.Item1.Start, pair.Item2)
            || IsInSequence(pair.Item2.Start, pair.Item1);
    }

    private static bool IsInSequence(long value, (long Start, long End) sequence)
    {
        return value >= sequence.Start && value <= sequence.End;
    }

    private static bool FullyContains((long Start, long End) sequence, (long Start, long End) otherSequence)
    {
        return (sequence.Start <= otherSequence.Start && sequence.End >= otherSequence.End);
    }
}