using Aoc;
using static Aoc.Helpers;

namespace Aoc2023;

public class Solver202303 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid = new Grid2D<char>(lines.ToArray2D());
        var result = grid.CoordEnumerable().Aggregate(
            new
            {
                Numbers = new List<Number>(),
                CurrentNumber = (Number?)null
            },
            (result, coord) =>
            {
                if (result.CurrentNumber is not null && (!char.IsDigit(grid[coord]) || coord.X == 0))
                {
                    result.Numbers.Add(result.CurrentNumber);
                }
                return new
                {
                    result.Numbers,
                    CurrentNumber = char.IsDigit(grid[coord])
                        ? Number.Add(coord.X == 0 ? null : result.CurrentNumber, ParseChar(grid[coord]), coord)
                        : null
                };
            }
        );
        if (result.CurrentNumber is not null)
        {
            result.Numbers.Add(result.CurrentNumber);
        }
        return (
            result.Numbers.Where(number => IsPartNumber(number, grid)).Sum(number => number.Value),
            result.Numbers.Aggregate(new DictionaryWithDefault<Point, ISet<Number>>(() => new HashSet<Number>()),
                (result, number) => {
                    foreach (var point in number.EnumerateNeighbours().Where(grid.IsInBounds))
                    {
                        if (grid[point] == '*') {
                            result[point].Add(number);
                        }
                    }
                    return result;
                })
                .Where(pair => pair.Value.Count == 2)
                .Select(pair => pair.Value.Select(number => number.Value).Product())
                .Sum()
        );
    }

    private static bool IsPartNumber(Number number, Grid2D<char> grid)
    {
        return grid.EnumerateValues(number.EnumerateNeighbours()).Any(c => c != '.' && !char.IsDigit(c));
    }
}

internal class Number
{
    public int Y { get; private set; }
    public int Start { get; private set; }
    public int End { get; private set; }
    public int Value { get; private set; }

    public Number(Point point, int value)
    {
        Y = point.Y;
        Start = End = point.X;
        Value = value;
    }

    public Number Add(int digit)
    {
        Value = Value * 10 + digit;
        End++;
        return this;
    }

    public IEnumerable<Point> EnumerateNeighbours()
    {
        for (int X = Start - 1; X <= End + 1; X++)
        {
            yield return new Point(X, Y - 1);
        }
        for (int X = Start - 1; X <= End + 1; X++)
        {
            yield return new Point(X, Y + 1);
        }
        yield return new Point(Start - 1, Y);
        yield return new Point(End + 1, Y);
        
    }

    internal static Number? Add(Number? currentNumber, int value, Point coord)
    {
        return currentNumber == null
            ? new Number(coord, value)
            : currentNumber.Add(value);
    }
}
