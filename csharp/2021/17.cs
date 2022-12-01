using System.Text.RegularExpressions;
using Aoc;

namespace Aoc2021;

public class Solver202117 : ISolver
{
    private readonly Regex inputRegex = new(@"target area: x=(-?\d+)..(-?\d+), y=(-?\d+)..(-?\d+)");
    public dynamic Solve(string[] lines)
    {
        var target = ParseInput(lines[0]);
        var simulations = Enumerable.Range(0, target.Xmax + 1).SelectMany(x =>
            Enumerable.Range(target.Ymin, target.Ymax - target.Ymin + target.Xmax)
                .Select(y => (x, y)))
            .Select(v => Fire(v.x, v.y, target))
            .Where(trajectory => trajectory.Count > 0);
        return (simulations.Flatten().Select(p => p.Y).Max(),
            simulations.Count());
    }

    private IList<Point> Fire(int dx, int dy, TargetArea target)
    {
        int x = 0, y = 0;
        IList<Point> trajectory = new List<Point>();
        while (x <= target.Xmax && y >= target.Ymin)
        {
            trajectory.Add(new Point(x, y));
            if (target.IsInBounds(x, y))
            {
                return trajectory;
            }
            x += dx;
            y += dy;
            if (dx > 0)
            {
                dx -= 1;
            }
            else if (dx < 0)
            {
                dx += 1;
            }
            dy -= 1;
        }
        return new List<Point>();
    }
    private TargetArea ParseInput(string input)
    {
        var m = inputRegex.Match(input);
        var coords = Enumerable.Range(1, 4).Select(i => m.Groups[i].Value).Select(int.Parse).ToArray();
        return new TargetArea(coords);
    }
}

class TargetArea
{
    public int Xmin { get; private set; }
    public int Xmax { get; private set; }
    public int Ymin { get; private set; }
    public int Ymax { get; private set; }

    public TargetArea(int[] coords)
    {
        Xmin = Math.Min(coords[0], coords[1]);
        Xmax = Math.Max(coords[0], coords[1]);
        Ymin = Math.Min(coords[2], coords[3]);
        Ymax = Math.Max(coords[2], coords[3]);
    }

    public bool IsInBounds(int x, int y)
    {
        return Xmin <= x && x <= Xmax
            && Ymin <= y && y <= Ymax;
    }
}