using Aoc;

namespace Aoc2021;

public class Solver202115 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid = new Grid2D<int>(lines.Select(line => line.AsEnumerable().Select(Helpers.ParseChar))
            .ToArray2D());
        var grid5 = new Grid2D<int>(5 * grid.Width, 5 * grid.Height);
        foreach (var p in grid.CoordEnumerable())
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    int value = grid.ValueAt(p) + x + y;
                    grid5.SetValueAt(new Point(x * grid.Width + p.X, y * grid.Height + p.Y),
                        value > 9 ? value - 9 : value);
                }
            }
        }
        return (LowestRiskPath(grid), LowestRiskPath(grid5));
    }

    private static int LowestRiskPath(Grid2D<int> grid)
    {
        return Graph.ShortestPath(
            new Point(0, 0),
            point => grid.AdjacentPoints(point, Grid2D.OrthogonalNeighbours),
            (_, neighbour) => grid[neighbour]
        )[grid.BottomRight];
    }
}