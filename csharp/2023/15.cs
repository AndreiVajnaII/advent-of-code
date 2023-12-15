using Aoc;
using static Aoc.Helpers;

namespace Aoc2023;

public class Solver202315 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var operations = lines[0].Split(",").ToArray();

        var boxes = InitializeImmutableArray<Box>(256);

        foreach (var operation in operations)
        {
            if (operation.Contains('='))
            {

                (var lens, var focal) = operation.Split('=').AsTuple2(s => s, int.Parse);
                var index = CalculateHash(lens);
                boxes[index].Add(lens, focal);
            }
            else
            {
                var lens = operation[0..^1];
                var index = CalculateHash(lens);
                boxes[index].Remove(lens);
            }
        }

        return (
            operations.Select(CalculateHash).Sum(),
            boxes.Select((box, i) => box.FocusingPower(i)).Sum()
        );
    }

    private int CalculateHash(string s)
    {
        int value = 0;
        foreach (var c in s)
        {
            value += c;
            value *= 17;
            value %= 256;
        }
        return value;
    }
}

internal class Box
{
    private readonly List<string> _lenses = [];
    private readonly Dictionary<string, int> _focalLengths = [];

    public void Add(string label, int focal)
    {
        if (!_focalLengths.ContainsKey(label))
        {
            _lenses.Add(label);
        }
        _focalLengths[label] = focal;
    }

    public int FocusingPower(int i)
    {
        return _lenses.Select((lens, j) => (i + 1) * (j + 1) * _focalLengths[lens]).Sum();
    }

    public void Remove(string label)
    {
        _lenses.Remove(label);
        _focalLengths.Remove(label);
    }
}
