public class Solver202109 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid = new Grid2D<int>(lines.Select(line =>
            line.AsEnumerable().Select(Helpers.ParseChar)).ToArray2D());
        var basins = Basins(grid);
        basins.Sort();
        return (
            LowestPoints(grid).Select(height => height + 1).Sum(),
            basins.TakeLast(3).Aggregate((a, b) => a * b)
        );
    }

    private IEnumerable<int> LowestPoints(Grid2D<int> grid)
    {
        return from p in grid.CoordEnumerable()
               where grid.Adjacents(p).All(adjValue => grid.ValueAt(p) < adjValue)
               select grid.ValueAt(p);
    }

    private List<int> Basins(Grid2D<int> grid)
    {
        var basins = new List<int>();
        var basinMap = new int?[grid.getHeight(), grid.getWidth()];
        foreach (var p in grid.CoordEnumerable().Where(p => grid.ValueAt(p) != 9))
        {
            var left = ValueOrNullAt(grid, basinMap, new Point(p.X - 1, p.Y));
            var up = ValueOrNullAt(grid, basinMap, new Point(p.X, p.Y - 1));
            if (left is null && up is null)
            {
                basinMap[p.Y, p.X] = basins.Count;
                basins.Add(1);
            }
            else if (left is null || up is null)
            {
                int basin = left.HasValue ? left.GetValueOrDefault() : up.GetValueOrDefault();
                basinMap[p.Y, p.X] = basin;
                basins[basin] = basins[basin] + 1;
            }
            else
            {
                int leftBasin = left.GetValueOrDefault();
                int upBasin = up.GetValueOrDefault();
                if (leftBasin != upBasin)
                {
                    basinMap[p.Y, p.X] = leftBasin;
                    basins[leftBasin] = basins[leftBasin] + basins[upBasin] + 1;
                    basins[upBasin] = 0;
                }
                else {
                    basinMap[p.Y, p.X] = leftBasin;
                    basins[leftBasin] = basins[leftBasin] + 1;
                }
            }
        }
        return basins;
    }

    private int? ValueOrNullAt(Grid2D<int> grid, int?[,] basinMap, Point p)
    {
        return grid.IsInBounds(p) ? basinMap[p.Y, p.X] : null;
    }
}