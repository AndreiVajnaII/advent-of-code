using Aoc;

namespace Aoc2023;

public class Solver202312 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        return (
            lines.Select(line => CountArrangements(line, 1)).Sum(),
            lines.Select(line => CountArrangements(line, 5)).Sum()
        );
    }

    private static long CountArrangements(string line, int times = 1)
    {
        var split = line.Split(" ");
        var springs = split[0];
        var criteria = split[1];
        for (int i = 1; i < times; i++)
        {
            springs += "?" + split[0];
            criteria += "," + split[1];
        }
        var checker = Parse(springs + " " + criteria);
        return checker.CountGoodArrangements();
    }

    private static HotSpringChecker Parse(string line)
    {
        (var springs, var criteria) = line.Split(" ").AsTuple2(
            s => s.ToCharArray(),
            s => s.Split(",").Select(int.Parse).ToArray()
        );
        return new HotSpringChecker(springs, criteria);
    }
}

internal class HotSpringChecker(char[] springs, int[] criteria)
{
    private readonly Dictionary<(int, int, int), long> _cache = [];

    public long CountGoodArrangements(int index = 0, int patternIndex = 0, int currentPattern = 0)
    {
        var key = (index, patternIndex, currentPattern);
        if (_cache.ContainsKey(key)) return _cache[key];

        long result = 0;
        if (index == springs.Length)
        {
            if (patternIndex == criteria.Length || (patternIndex == criteria.Length - 1 && currentPattern == criteria[patternIndex]))
            {
                result = 1;
            }
        }
        else
        {
            if (springs[index] == '?')
            {
                if (currentPattern == 0 || criteria[patternIndex] == currentPattern)
                {
                    springs[index] = '.';
                    result += CountGoodArrangements(index + 1, currentPattern > 0 ? patternIndex + 1 : patternIndex, 0);
                }
                if (patternIndex < criteria.Length && currentPattern + 1 <= criteria[patternIndex])
                {
                    springs[index] = '#';
                    result += CountGoodArrangements(index + 1, patternIndex, currentPattern + 1);
                }
                springs[index] = '?';
            }
            else
            {
                if (springs[index] == '.')
                {
                    if (currentPattern == 0 || criteria[patternIndex] == currentPattern)
                    {
                        result += CountGoodArrangements(index + 1, currentPattern > 0 ? patternIndex + 1 : patternIndex, 0);
                    }
                }
                else
                {
                    if (patternIndex < criteria.Length && currentPattern + 1 <= criteria[patternIndex])
                    {
                        result += CountGoodArrangements(index + 1, patternIndex, currentPattern + 1);
                    }
                }
            }
        }

        _cache[key] = result;
        return result;
    }
}