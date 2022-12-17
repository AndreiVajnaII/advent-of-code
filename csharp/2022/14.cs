using Aoc;

namespace Aoc2022;

public class Solver202214 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid = new SparseGrid<bool>(false);
        foreach (var path in lines.Select(Parse))
        {
            foreach (var (start, end) in path.ZipShift())
            {
                foreach (var point in start.NavigateTo(end))
                {
                    grid[point] = true;
                }
            }
        }
        var gridWithFloor = new SparseGridWithFloor(grid);
        var part1 = DropSand(grid, restPosition => restPosition.Y >= grid.Ymax) - 1;
        var part2 = DropSand(gridWithFloor, restPosition => restPosition == new Point(500, 0));

        return (
            part1,
            part1 + part2
        );
    }

    private static int DropSand(IPointGrid<bool> grid, Func<Point, bool> endCondition)
    {
        Point restPosition;
        var sandUnits = 0;
        do
        {
            sandUnits++;
            restPosition = DropUnitOfSand(grid);
            grid[restPosition] = true;
        } while (!endCondition(restPosition));
        return sandUnits;
    }

    private static Point DropUnitOfSand(IPointGrid<bool> grid)
    {
        return new Point(500, 0).Navigate(point => MoveSand(point, grid))
            .TakeWhile(point => point.Y <= grid.Ymax)
            .Last();
    }

    private static Point? MoveSand(Point point, IPointGrid<bool> grid)
    {
        return point.SelectAdjacents(new[] { (0, 1), (-1, 1), (1, 1) })
            .FirstOrNull(adjacent => grid[adjacent] == false);
    }

    private static IEnumerable<Point> Parse(string line)
    {
        return line.Split(" -> ").Select(Point.Parse);
    }

    private class SparseGridWithFloor : IPointGrid<bool>
    {
        private readonly IPointGrid<bool> grid;

        public SparseGridWithFloor(SparseGrid<bool> grid)
        {
            this.grid = grid;
            grid.Ymax += 2;
        }

        public bool this[Point point]
        {
            get => point.Y == Ymax || grid[point];
            set => grid[point] = value;
        }

        public int Xmin => grid.Xmin;
        public int Xmax => grid.Xmax;
        public int Ymin => grid.Ymin;
        public int Ymax => grid.Ymax;
    }
}