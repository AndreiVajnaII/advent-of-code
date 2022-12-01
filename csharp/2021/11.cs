using Aoc;

namespace Aoc2021;

public class Solver202111 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid = new Grid2D<int>(lines.Select(line => line.AsEnumerable().Select(Helpers.ParseChar))
            .ToArray2D());
        return (
            Enumerable.Range(1, 100).Sum(_ => Step(grid)),
            Enumerable.Range(101, int.MaxValue - 100).TakeWhile(_ => Step(grid) != grid.Count).Last() + 1
        );
    }

    private int Step(Grid2D<int> grid)
    {
        int flashes = 0;
        var toFlash = new Queue<Point>();
        foreach (var p in grid.CoordEnumerable())
        {
            grid.SetValueAt(p, grid.ValueAt(p) + 1);
            if (grid.ValueAt(p) == 10)
            {
                toFlash.Enqueue(p);
            }
        }
        while (toFlash.Count > 0)
        {
            var flashPoint = toFlash.Dequeue();
            foreach (var adjacent in grid.AdjacentPoints(flashPoint, Grid2D.allNeighbours))
            {
                if (grid.ValueAt(adjacent) > 0)
                {
                    grid.SetValueAt(adjacent, grid.ValueAt(adjacent) + 1);
                    if (grid.ValueAt(adjacent) == 10)
                    {
                        toFlash.Enqueue(adjacent);
                    }
                }
            }
            flashes++;
            grid.SetValueAt(flashPoint, 0);
        }
        return flashes;
    }
}