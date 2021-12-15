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
        var totals = grid.Spawn<int?>();
        var queue = new HashSet<Point>() { new Point(0, 0) };
        while (queue.Count > 0)
        {
            var newQueue = new HashSet<Point>();
            foreach (var p in queue)
            {
                totals.SetValueAt(p, grid.ValueAt(p) +
                    totals.Adjacents(p, Grid2D.orthogonalNeighbours)
                        .Where(v => v is not null).Min().GetValueOrDefault(0));
                foreach (var neighbour in totals.AdjacentPoints(p, Grid2D.orthogonalNeighbours)
                    .Where(adj => totals.ValueAt(adj) is null
                        || totals.ValueAt(adj) > totals.ValueAt(p) + grid.ValueAt(adj)))
                {
                    newQueue.Add(neighbour);
                }
            }
            queue = newQueue;
        }
        return totals.ValueAt(totals.BottomRight)!.Value - grid.ValueAt(new Point(0, 0));
    }
}