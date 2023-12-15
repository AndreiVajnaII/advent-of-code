using Aoc;

namespace Aoc2023;

public class Solver202314 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid1 = new Grid2D<char>(lines.ToArray2D());
        var grid2 = new Grid2D<char>(lines.ToArray2D());

        Roll(grid1, grid1.ColumnEnumerable());

        var history = new List<(int Load, string Grid)>();
        var spins = 0;
        var start = 0;
        while (true)
        {
            Spin(grid2);
            var load = CalculateLoad(grid2);
            var existing = history.WithIndex().FirstOrNull(entry => entry.Item.Load == load && entry.Item.Grid == grid2.ToString(""));
            if (existing is not null) {
                start = existing.Value.Index;
                break;
            }
            history.Add((load, grid2.ToString("")));
            spins++;
        }

        return (
            CalculateLoad(grid1),
            history[start - 1 + (1000000000 - start) % (spins - start)].Load
        );
    }

    private static int CalculateLoad(Grid2D<char> grid)
        => grid.CoordEnumerable()
            .Where(point => grid[point] == 'O')
            .Sum(point => grid.Height - point.Y);

    private static void Spin(Grid2D<char> grid)
    {
        Roll(grid, grid.ColumnEnumerable());
        Roll(grid, grid.GridEnumerable());
        Roll(grid, grid.ColumnEnumerable().Select(column => column.Reverse()));
        Roll(grid, grid.GridEnumerable().Select(row => row.Reverse()));
    }

    private static void Roll(Grid2D<char> grid, IEnumerable<IEnumerable<Point>> lineEnumerable)
    {
        foreach (var line in lineEnumerable)
        {
            Point? prevEmptySpot = null;
            int prevEmptySpotIndex = 0;
            var lineArray = line.ToArray();
            for (int i = 0; i < lineArray.Length; i++)
            {
                var point = lineArray[i];
                switch (grid[point])
                {
                    case '.':
                        if (prevEmptySpot == null)
                        {
                            prevEmptySpot = point;
                            prevEmptySpotIndex = i;
                        }
                        break;
                    case '#':
                        prevEmptySpot = null;
                        break;
                    case 'O':
                        if (prevEmptySpot != null)
                        {
                            grid[point] = '.';
                            grid[prevEmptySpot.Value] = 'O';
                            prevEmptySpotIndex++;
                            prevEmptySpot = lineArray[prevEmptySpotIndex];
                        }
                        break;
                }
            }
        }
    }
}