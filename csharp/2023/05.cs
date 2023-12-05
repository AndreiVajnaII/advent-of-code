using Aoc;
using static Aoc.Helpers;

namespace Aoc2023;

public class Solver202305 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var seeds = lines[0].Split(": ")[1].Split(" ").Select(long.Parse);
        var seedRanges = seeds.TakeEvery(2, 0).Zip(seeds.TakeEvery(2, 1)).ToList();
        var maps = GroupLines(lines).Skip(1).Select(ParseMap).ToArray();
        return (
            seeds.Select(ApplyMaps(maps)).Min(),
            seedRanges.SelectMany(ApplyMapsForRange(maps)).Min(range => range.Start)
        );
    }

    private static Func<long, long> ApplyMaps(Map[] maps) => seed =>
    {
        var value = seed;
        foreach (var map in maps)
        {
            value = map.Apply(value);
        }
        return value;
    };

    private static Func<(long, long), List<(long Start, long Length)>> ApplyMapsForRange(Map[] maps) => seedRange =>
    {
        var ranges = new List<(long, long)> { seedRange };
        foreach (var map in maps)
        {
            ranges = ranges.SelectMany(map.Apply).ToList();
        }
        return ranges;
    };

    private Map ParseMap(IEnumerable<string> group)
    {
        var ranges = group.Skip(1).Select(ParseRange).ToList();
        ranges.Sort(Range.Compare);
        long prevEnd = 0;
        for (int i = 0; i < ranges.Count; i++)
        {
            if (ranges[i].SrcStart - prevEnd > 0)
            {
                ranges.Insert(i, new Range([prevEnd, prevEnd, ranges[i].SrcStart - prevEnd]));
                i++;
            }
            prevEnd = ranges[i].SrcStart + ranges[i].Length;
        }
        return new Map(ranges);
    }

    private Range ParseRange(string line)
    {
        return new Range(line.Split(" ").Select(long.Parse).ToArray());
    }
}

internal class Map(List<Range> ranges)
{
    private List<Range> _ranges = ranges;

    public long Apply(long value)
    {
        var range = _ranges.FirstOrDefault(range => range.Contains(value));
        return range is null ? value : range.Apply(value);
    }

    public IEnumerable<(long, long)> Apply((long Start, long Length) seedRange)
    {
        int i = 0;
        for (; i < _ranges.Count; i++)
        {
            if (_ranges[i].Contains(seedRange.Start))
            {
                break;
            }
        }
        var remaining = seedRange.Length;
        var start = seedRange.Start;
        while (i < _ranges.Count && remaining > 0)
        {
            var result = _ranges[i].Apply(start, remaining);
            yield return result;
            remaining -= result.Length;
            start += result.Length;
            i++;
        }
        if (remaining > 0) {
            yield return (start, remaining);
        }
    }
}

internal class Range(long[] numbers)
{
    private long _srcStart = numbers[1];
    private long _destStart = numbers[0];
    private long _length = numbers[2];

    public long SrcStart { get => _srcStart; }
    public long DestStart { get => _destStart; }
    public long Length { get => _length; }

    public bool Contains(long value)
    {
        return value >= SrcStart && value < SrcStart + Length;
    }

    public long Apply(long value)
    {
        return _destStart + (value - SrcStart);
    }

    public static int Compare(Range x, Range y)
    {
        return y.SrcStart == x.SrcStart ? 0 : y.SrcStart > x.SrcStart ? -1 : 1;
    }

    public (long Start, long Length) Apply(long start, long remaining)
    {
        return (
            Apply(start),
            start + remaining < SrcStart + Length
                ? remaining
                : SrcStart + Length - start
        );
    }
}