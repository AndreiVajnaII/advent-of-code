using Aoc;

namespace Aoc2021;

public class Solver202125 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid = new Grid2D<char>(lines.ToArray2D());
        var eastCucumber = new EastCucumber();
        var southCucumber = new SouthCucumber();
        int moves = 0;
        int steps = 0;
        do
        {
            var eastMoves = ComputeMoves(grid, eastCucumber).ToArray();
            foreach (var move in eastMoves)
            {
                eastCucumber.Move(grid, move);
            }
            var southMoves = ComputeMoves(grid, southCucumber).ToArray();
            foreach (var move in southMoves)
            {
                southCucumber.Move(grid, move);
            }
            moves = eastMoves.Length + southMoves.Length;
            steps++;
        }
        while (moves > 0);
        return steps;
    }

    private IEnumerable<Point> ComputeMoves(Grid2D<char> grid, Cucumber cucumber)
    {
        foreach (var p in grid.CoordEnumerable())
        {
            if (grid.ValueAt(p) == cucumber.Type)
            {
                if (grid.ValueAt(cucumber.Neighbour(p, grid)) == '.')
                {
                    yield return p;
                }
            }
        }
    }
}

abstract class Cucumber
{
    public char Type { get; private set; }

    public Cucumber(char type)
    {
        Type = type;
    }

    public void Move(Grid2D<char> grid, Point p)
    {
        grid.SetValueAt(p, '.');
        grid.SetValueAt(Neighbour(p, grid), Type);
    }

    public abstract Point Neighbour(Point p, Grid2D<char> grid);
}

class EastCucumber : Cucumber
{
    public EastCucumber() : base('>') { }

    public override Point Neighbour(Point p, Grid2D<char> grid)
        => new Point(p.X + 1 == grid.Width ? 0 : p.X + 1, p.Y);
}

class SouthCucumber : Cucumber
{
    public SouthCucumber() : base('v') { }

    public override Point Neighbour(Point p, Grid2D<char> grid)
        => new Point(p.X, p.Y + 1 == grid.Height ? 0 : p.Y + 1);
}