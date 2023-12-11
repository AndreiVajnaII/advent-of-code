using Aoc;
using static Aoc.Graph;
using static Aoc.Grid2D;

namespace Aoc2023;

using WalkState = (Point Point, Direction Direction);

public class Solver202310 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var maze = new Grid2D<char>(lines.ToArray2D());
        var start = maze.PositionOf('S');

        var startDirection = OrthogonalNeighbours
            .Where(direction => maze.IsInBounds(start.Move(direction)))
            .First(direction => CanEnterCell(maze[start.Move(direction)], direction));

        var steps = Walk(
            (WalkState)(start.Move(startDirection), startDirection),
            current => current.Point == start,
            current => Move(current.Point, current.Direction, maze)
        ).Select(step => step.Point).Prepend(start).ToHashSet();

        var minX = steps.Min(step => step.X);
        var minY = steps.Min(step => step.Y);
        var maxX = steps.Max(step => step.X);
        var maxY = steps.Max(step => step.Y);

        maze[start] = GetConnector(start, maze);

        var tiles = 0;
        for (int y = minY; y <= maxY; y++)
        {
            var insideX = false;
            for (int x = minX; x <= maxX; x++)
            {
                var point = new Point(x, y);
                if (steps.Contains(point))
                {
                    switch (maze[point])
                    {
                        case '|':
                            insideX = !insideX;
                            break;
                        case '-':
                            break;
                        case 'J':
                            break;
                        case 'F':
                            insideX = !insideX;
                            break;
                        case '7':
                            insideX = !insideX;
                            break;
                        case 'L':
                            break;
                    }
                }
                else if (insideX)
                {
                    tiles++;
                }
            }
        }

        return (steps.Count / 2, tiles);
    }

    private static char GetConnector(Point start, Grid2D<char> maze)
    {
        var north = CanEnterCell(maze[start.Move(North)], North);
        var south = CanEnterCell(maze[start.Move(South)], South);
        var east = CanEnterCell(maze[start.Move(East)], East);
        var west = CanEnterCell(maze[start.Move(West)], West);
        if (north && south) return '|';
        if (north && east) return 'L';
        if (north && west) return 'J';
        if (east && west) return '-';
        if (east && south) return 'F';
        if (west && south) return '7';
        throw new ArgumentException("Inavlid state");
    }

    private static bool CanEnterCell(char cell, (int dX, int dY) direction) => cell switch
    {
        '|' => direction == North || direction == South,
        '-' => direction == East || direction == West,
        'L' => direction == South || direction == West,
        'J' => direction == South || direction == East,
        '7' => direction == East || direction == North,
        'F' => direction == West || direction == North,
        _ => false
    };

    private static WalkState Move(Point point, Direction direction, Grid2D<char> maze)
    {
        var newDirection = GetNewDirection(maze[point], direction);
        return (point.Move(newDirection), newDirection);
    }

    public static Direction GetNewDirection(char cell, Direction direction) => cell switch
    {
        '|' => direction,
        '-' => direction,
        'L' => direction == South ? East : North,
        'J' => direction == South ? West : North,
        '7' => direction == East ? South : West,
        'F' => direction == West ? South : East,
        _ => throw new ArgumentException("Cannot move through cell " + cell)
    };
}
