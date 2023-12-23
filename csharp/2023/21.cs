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
        var steps = Walk(start, 0, grid).ToArray();

        const long stepCount = 26501365;
        // const long stepCount = 11;
        var radius = (stepCount - start.X - 1) / grid.Width;
        var partialGrids = 4 * (radius + 1) ;
        
        var top = Walk(new Point(start.X, grid.Ymax), grid.Width * radius + start.X + 1, grid)
                .Where(state => state.Steps <= stepCount).Count();
        var left = Walk(new Point(grid.Xmax, start.Y), grid.Width * radius + start.X + 1, grid)
                .Where(state => state.Steps <= stepCount).Count();
        var bottom = Walk(new Point(start.X, 0), grid.Width * radius + start.X + 1, grid)
                .Where(state => state.Steps <= stepCount).Count();
        var right = Walk(new Point(0, start.Y), grid.Width * radius + start.X + 1, grid)
                .Where(state => state.Steps <= stepCount).Count();
        
        var topLeft = Walk(new Point(grid.Xmax, grid.Ymax), grid.Width * radius + 1, grid)
            .Where(state => state.Steps <= stepCount).Count();
        var topLeft2 = Walk(new Point(grid.Xmax, grid.Ymax), grid.Width * (radius + 1) + 1, grid)
            .Where(state => state.Steps <= stepCount).Count();
        var topRight = Walk(new Point(0, grid.Ymax), grid.Width * radius + 1, grid)
            .Where(state => state.Steps <= stepCount).Count();
        var topRight2 = Walk(new Point(0, grid.Ymax), grid.Width * (radius + 1) + 1, grid)
            .Where(state => state.Steps <= stepCount).Count();
        var bottomLeft = Walk(new Point(grid.Xmax, 0), grid.Width * radius + 1, grid)
            .Where(state => state.Steps <= stepCount).Count();
        var bottomLeft2 = Walk(new Point(grid.Xmax, 0), grid.Width * (radius + 1) + 1, grid)
            .Where(state => state.Steps <= stepCount).Count();
        var bottomRight = Walk(new Point(0, 0), grid.Width * radius + 1, grid)
            .Where(state => state.Steps <= stepCount).Count();
        var bottomRight2 = Walk(new Point(0, 0), grid.Width * (radius + 1) + 1, grid)
            .Where(state => state.Steps <= stepCount).Count();


        // var cornerWalk = Walk(new Point(0, 0), grid.Width * radius + 1, grid);
        // var cornerGrid = grid.Spawn<string>();
        // foreach(var point in grid.CoordEnumerable())
        // {
        //     cornerGrid[point] = grid[point] == '#' ? " #" : ".";
        // }
        // foreach (var state in cornerWalk)
        // {
        //     cornerGrid[state.Point] = Helpers.PadDay(state.Steps.ToString());
        // }
        // // Console.WriteLine(cornerGrid.ToString(" "));
        // // Console.WriteLine();
        // var cornerGridStepCount =
        //     cornerWalk
        //         .Where(state => state.Steps <= stepCount)
        //         .Count();
        
        var gridPlots = grid.ValueEnumerable().Count(value => value == '.');
        var totalPlots =  (2 * radius * radius + 2 * radius + 1) * gridPlots +
            + top + left + bottom + right
            + radius * (topLeft + topRight + bottomLeft + bottomRight)
            + (radius + 1) * (topLeft2 + topRight2 + bottomLeft2 + bottomRight2);


        // var newlines = new string[lines.Length * 5];
        // Array.Fill(newlines, "");
        // for (int i = 0; i < 5; i++)
        // {
        //     for (int j = 0; j < lines.Length; j++)
        //     {
        //         for (int k = 0; k < 5; k++)
        //         {
        //             newlines[i * lines.Length + j] += lines[j].Replace("S", ".");
        //         }
        //     }
        // }
        // var extendedGrid = newlines.ToGrid();
        // var extendedWalk = Walk(new Point(extendedGrid.Width / 2, extendedGrid.Height / 2), 0, extendedGrid).ToArray();
        // var stepGrid = extendedGrid.Spawn<string>();
        // foreach(var point in extendedGrid.CoordEnumerable())
        // {
        //     stepGrid[point] = extendedGrid[point] == '#' ? " #" : ".";
        // }
        // foreach (var state in extendedWalk)
        // {
        //     stepGrid[state.Point] = Helpers.PadDay(state.Steps.ToString());
        // }
        // Console.WriteLine(stepGrid.ToString(" "));
        // var actualPlots = extendedWalk
        //     .Where(state => state.Steps <= stepCount)
        //     .Count();

        return (
            steps
                .Where(state => state.Steps <= 64 && state.Steps % 2 == 0)
                .Count(),
            totalPlots
        );
    }

    private static IEnumerable<(Point Point, long Steps)> Walk(Point start, long startSteps, Grid2D<char> grid)
    {
        var visited = new HashSet<Point>() { start };
        (Point Point, long Steps) initialState = (start, startSteps);
        return Graph.Traverse(initialState,
            state => grid.AdjacentPoints(state.Point, OrthogonalNeighbours)
                .Where(point => grid[point] == '.')
                .Where(point => !visited.Contains(point))
                .Do(point => visited.Add(point))
                .Select(point => (point, state.Steps + 1)));
    }
}