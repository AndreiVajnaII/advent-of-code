using Aoc;
using static Aoc.Grid2D;

namespace Aoc2023;

public class Solver202323 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var linesWithoutSlopes = lines.Select(line => line
            .Replace('>', '.')
            .Replace('v', '.')
            .Replace('<', '.')
            .Replace('^', '.'))
            .ToArray();
        return (
            Part1(lines),
            Part2(linesWithoutSlopes)
        );
    }

    private static int Part1(string[] lines)
    {
        var grid = lines.ToGrid();
        var start = grid.GridEnumerable().First().Single(point => grid[point] == '.');
        var end = grid.GridEnumerable().Last().Single(point => grid[point] == '.');
        return WalkGrid(grid, start, end);
    }

    private static int Part2(string[] lines)
    {
        var grid = lines.ToGrid();
        var start = grid.GridEnumerable().First().Single(point => grid[point] == '.');
        var end = grid.GridEnumerable().Last().Single(point => grid[point] == '.');
        var graph = BuildJunctionGraph(start, grid);
        return WalkGraph(graph, start, end, []);
    }

    private static int WalkGrid(Grid2D<char> grid, Point start, Point end)
    {
        var results = new Dictionary<Point, int> { [end] = 0 };
        var visited = new HashSet<Point>();
        var stack = new Stack<Point>();
        stack.Push(start);

        while (stack.TryPop(out var current))
        {
            var isAwaitingResult = visited.Contains(current);
            visited.Add(current);
            var neighbours = OrthogonalNeighbours
                .Select(direction => (Point: current.Move(direction), Direction: direction))
                .Where(pair => grid.IsInBounds(pair.Point))
                .Where(pair => !visited.Contains(pair.Point))
                .Where(pair => grid[pair.Point] == '.'
                    || (grid[pair.Point] != '#' && pair.Direction == DirectionOf(grid[pair.Point])))
                .Select(pair => pair.Point);

            var unvisited = neighbours.Where(neighbour => !results.ContainsKey(neighbour)).ToArray();
            if (unvisited.Length == 0)
            {
                results[current] = !neighbours.Any() ? 0
                    : 1 + neighbours.Max(neighbour => results[neighbour]);
                visited.Remove(current);
            }
            else
            {
                stack.Push(current);
                foreach (var neighbour in neighbours)
                {
                    stack.Push(neighbour);
                }
            }
        }
        return results[start];
    }

    private static int WalkGraph(DictionaryWithDefault<Point, Dictionary<Point, int>> graph, Point current, Point end, HashSet<Point> visited)
    {
        if (current == end) return 0;
        visited.Add(current);
        var neighbours = graph[current]
            .Where(entry => !visited.Contains(entry.Key))
            .ToArray();
        var result = neighbours.Length == 0 ? 0
            : neighbours.Max(entry => entry.Value + WalkGraph(graph, entry.Key, end, visited));
        visited.Remove(current);
        return result;
    }

    private static DictionaryWithDefault<Point, Dictionary<Point, int>> BuildJunctionGraph(Point start, Grid2D<char> grid)
    {
        var junctionGraph = new DictionaryWithDefault<Point, Dictionary<Point, int>>(() => []);
        var visited = new HashSet<Point> { start };

        var states = new Queue<(Point Point, Point neightbour)>();
        states.Enqueue((start, GetNeighbours(start, grid).Single()));
        while (states.Count > 0)
        {
            (var point, var neighbour) = states.Dequeue();
            if (visited.Contains(neighbour)) continue;
            var steps = 1;
            while (!IsJunction(neighbour, grid))
            {
                visited.Add(neighbour);
                steps++;
                neighbour = GetNeighbours(neighbour, grid)
                    .Where(p => !visited.Contains(p) || (IsJunction(p, grid) && p != point))
                    .Single();
            }
            junctionGraph[point][neighbour] = junctionGraph[neighbour][point] = steps;
            if (!visited.Contains(neighbour))
            {
                foreach (var newNeighbour in GetNeighbours(neighbour, grid).Where(p => !visited.Contains(p)))
                {
                    states.Enqueue((neighbour, newNeighbour));
                }
                visited.Add(neighbour);
            }
        }
        return junctionGraph;
    }

    private static IEnumerable<Point> GetNeighbours(Point point, Grid2D<char> grid)
        => grid.AdjacentPoints(point, OrthogonalNeighbours)
            .Where(p => grid[p] == '.');

    private static bool IsJunction(Point point, Grid2D<char> grid)
        => grid.Adjacents(point, OrthogonalNeighbours).Where(cell => cell == '.').Count() > 2
            || point.Y == grid.Ymax;

    private static Direction DirectionOf(char slope) => slope switch
    {
        '>' => East,
        'v' => South,
        '<' => West,
        '^' => North,
        _ => throw new ArgumentException("Invalid slope " + slope)
    };
}