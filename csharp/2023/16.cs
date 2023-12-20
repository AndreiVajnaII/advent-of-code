using Aoc;
using static Aoc.Grid2D;

namespace Aoc2023;

using Beam = (Point Position, Direction Direction);

public class Solver202316 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid = new Grid2D<char>(lines.ToArray2D());
        return (
            CountEnergizedTiles((new Point(0, 0), East), grid),
            Enumerable.Range(grid.Ymin, grid.Height).Select(y => (new Point(grid.Xmin, y), East)).Concat(
                Enumerable.Range(grid.Ymin, grid.Height).Select(y => (new Point(grid.Xmax, y), West))).Concat(
                Enumerable.Range(grid.Xmin, grid.Width).Select(x => (new Point(x, grid.Ymin), South))).Concat(
                Enumerable.Range(grid.Xmin, grid.Width).Select(x => (new Point(x, grid.Ymax), North)))
            .Select(startBeam => CountEnergizedTiles(startBeam, grid))
            .Max()
        );
    }

    private static int CountEnergizedTiles(Beam startBeam, Grid2D<char> grid)
    {
        return Graph.Traverse(startBeam, MoveBeam(grid))
            .Select(beam => beam.Position)
            .ToHashSet()
            .Count;
    }

    private static Func<Beam, IEnumerable<Beam>> MoveBeam(Grid2D<char> grid) => beam
        => MoveBeam(beam, grid[beam.Position])
            .Where(newBeam => grid.IsInBounds(newBeam.Position));

    private static IEnumerable<Beam> MoveBeam(Beam beam, char tile) => tile switch
    {
        '.' => [MoveBeam(beam.Position, beam.Direction)],
        '|' => beam.Direction.dX == 0
                        ? [MoveBeam(beam.Position, beam.Direction)]
                        : [MoveBeam(beam.Position, North), MoveBeam(beam.Position, South)],
        '-' => beam.Direction.dY == 0
                        ? [MoveBeam(beam.Position, beam.Direction)]
                        : [MoveBeam(beam.Position, East), MoveBeam(beam.Position, West)],
        '/' => [TurnBeamSlash(beam)],
        '\\' => [TurnBeamBackSlash(beam)],
        _ => throw new ArgumentException("Invalid tile : " + tile),
    };

    private static Beam TurnBeamSlash(Beam beam)
    {
        if (beam.Direction == North) return MoveBeam(beam.Position, East);
        if (beam.Direction == South) return MoveBeam(beam.Position, West);
        if (beam.Direction == East) return MoveBeam(beam.Position, North);
        if (beam.Direction == West) return MoveBeam(beam.Position, South);
        throw new ArgumentException("Invalid direction: " + beam.Direction);
    }

    private static Beam TurnBeamBackSlash(Beam beam)
    {
        if (beam.Direction == North) return MoveBeam(beam.Position, West);
        if (beam.Direction == South) return MoveBeam(beam.Position, East);
        if (beam.Direction == East) return MoveBeam(beam.Position, South);
        if (beam.Direction == West) return MoveBeam(beam.Position, North);
        throw new ArgumentException("Invalid direction: " + beam.Direction);
    }

    private static Beam MoveBeam(Point position, Direction direction)
        => (position.Move(direction), direction);
}