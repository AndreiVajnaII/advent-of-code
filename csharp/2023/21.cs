using Aoc;

using static Aoc.Grid2D;

namespace Aoc2023;

public class Solver202321 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid = lines.ToGrid();
        var start = grid.PositionOf('S');
        grid[start] = '.';
        return 0.EnumerateTo(64, inclusive: true).TakeEvery(2, 0)
            .SelectMany(steps => Walk(start, 0, steps, grid, []))
            .ToHashSet()
            .Count;
    }

    private readonly Dictionary<Point, int> visited = [];

    private static HashSet<Point> Walk(Point current, int steps, int end, Grid2D<char> grid, Dictionary<Point, int> visited)
    {
        visited[current] = steps;
        if (steps == end) return [current];

        var result = new HashSet<Point>();
        var neighbours = grid.AdjacentPoints(current, OrthogonalNeighbours)
            .Where(point => grid[point] == '.')
            .Where(point => !visited.ContainsKey(point) || (steps + 1) < visited[point]);
        foreach (var newPoint in neighbours)
        {
            foreach (var endPoint in Walk(newPoint, steps + 1, end, grid, visited))
            {
                result.Add(endPoint);
            }
        }
        return result;
    }
}