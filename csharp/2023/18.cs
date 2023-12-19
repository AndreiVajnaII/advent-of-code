using Aoc;
using static Aoc.Grid2D;
using static Aoc.Helpers;

namespace Aoc2023;

public class Solver202318 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var instructions1 = lines.Select(ParseDigInstruction1).ToArray();
        var instructions2 = lines.Select(ParseDigInstruction2).ToArray();
        return CalculateArea(instructions2);
    }

    private static long CalculateArea(IEnumerable<Instruction> instructions)
    {
        return CalculateArea(BuildLines(instructions));
    }

    private static long CalculateArea(IEnumerable<VerticalLine> enumerableLines)
    {
        var lines = enumerableLines.OrderBy(line => line.X).ThenBy(line => line.Start).ToArray();
        var ys = lines.Select(line => line.Start).Concat(lines.Select(line => line.End))
            .Distinct().Order().ToArray();

        var pins = ys.Select(y => lines.Where(line => line.Contains(y))
            .Select(line => line.CreatePin(y)));

        return pins.ZipShift().Select(CalculateArea).Sum();
    }

    private static long CalculateArea((IEnumerable<Pin>, IEnumerable<Pin>) tuple)
    {
        var pairs = tuple.Item1.Where(pin => pin.IsStart).Zip(tuple.Item2.Where(pin => pin.IsEnd)).ToArray();

        long area = 0;
        for (int i = 0; i < pairs.Length; i += 2)
        {
            area += (pairs[i].Second.Y - pairs[i].First.Y) * (pairs[i + 1].First.X - pairs[i].First.X);
        }
        return area;
    }

    private static IEnumerable<VerticalLine> BuildLines(IEnumerable<Instruction> instructions)
    {
        var point = new Point(0, 0);
        foreach (var instruction in instructions)
        {
            var line = instruction.BuildLine(ref point);
            if (line is not null) yield return line;
        }
    }

    private Instruction ParseDigInstruction1(string line)
    {
        var split = line.Split(" ");
        return new Instruction(split[0][0] switch
        {
            'R' => East,
            'D' => South,
            'L' => West,
            'U' => North,
            _ => throw new ArgumentException("Invalid direction " + split[0][0])
        },
            int.Parse(split[1]));
    }

    private Instruction ParseDigInstruction2(string line)
    {
        var split = line.Split(" ");
        var hex = split[2][2..^1];
        return new Instruction(hex[^1] switch
        {
            '0' => East,
            '1' => South,
            '2' => West,
            '3' => North,
            _ => throw new ArgumentException("Invalid direction " + hex[^1])
        },
            ParseHex(hex[0..^1]));
    }
}

internal class Instruction(Direction direction, int length)
{
    internal VerticalLine? BuildLine(ref Point point)
    {
        if (direction.dX == 0)
        {
            var dY = direction.dY * length;
            var line = dY < 0
                ? new VerticalLine(point.X, point.Y + dY, point.Y)
                : new VerticalLine(point.X, point.Y, point.Y + dY);
            point = point.Delta(0, dY);
            return line;
        }
        else
        {
            point = point.Delta(direction.dX * length, 0);
            return null;
        }
    }
}

internal class VerticalLine(int x, int start, int end)
{
    public int X { get; } = x;
    public int Start { get; } = start;
    public int End { get; } = end;

    public bool Contains(int y) => Start <= y && y <= End;

    public Pin CreatePin(int y)
    {
        return new Pin
        {
            X = X,
            Y = y,
            IsStart = y < End,
            IsEnd = y > Start
        };
    }
}

internal class Pin
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }

    public override string ToString()
    {
        var s = $"({X},{Y})";
        s += IsStart ? "s" : " ";
        s += IsEnd ? "e" : " ";
        return s;
    }
}
