using System.Collections.Immutable;
using Aoc;
using static Aoc.Helpers;

namespace Aoc2023;

public class Solver202322 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var bricks = lines.Select(Brick.Parse).OrderBy(brick => brick.Start.Z).ToArray();
        var canBeDesintegrated = new bool[bricks.Length];
        Array.Fill(canBeDesintegrated, true);
        var supports = InitializeImmutableArray<HashSet<int>>(bricks.Length);
        var supportedBy = InitializeImmutableArray<HashSet<int>>(bricks.Length);
        var zLayers = InitializeImmutableArray<Layer>(bricks.Max(brick => brick.End.Z) + 1);
        for (int i = 0; i < bricks.Length; i++)
        {
            var brick = bricks[i];
            while (brick.Start.Z > 1)
            {
                var bricksBelow = BricksBelow(zLayers[brick.Start.Z - 1], brick).ToArray();
                if (bricksBelow.Length == 0)
                {
                    brick.Start.Z--;
                    brick.End.Z--;
                }
                else
                {
                    if (bricksBelow.Length == 1)
                    {
                        canBeDesintegrated[bricksBelow[0]] = false;
                    }
                    foreach (var brickBelow in bricksBelow)
                    {
                        supports[brickBelow].Add(i);
                        supportedBy[i].Add(brickBelow);
                    }
                    break;
                }
            }
            for (int z = brick.Start.Z; z <= brick.End.Z; z++)
            {
                for (int x = brick.Start.X; x <= brick.End.X; x++)
                {
                    for (int y = brick.Start.Y; y <= brick.End.Y; y++)
                    {
                        zLayers[z][new Point(x, y)] = i;
                    }
                }
            }
        }

        return (
            canBeDesintegrated.Count(c => c == true),
            Enumerable.Range(0, bricks.Length)
                .Where(i => canBeDesintegrated[i] == false)
                .Select(i => FallingBrickCount([i], supports, supportedBy) - 1)
                .Sum()
        );
    }

    private static int FallingBrickCount(HashSet<int> destroyed, ImmutableArray<HashSet<int>> supports, ImmutableArray<HashSet<int>> supportedBy)
    {
        if (destroyed.Count == 0) return 0;

        var allSupported = destroyed.SelectMany(i => supports[i]).ToHashSet();
        var toAddBack = new DictionaryWithDefault<int, HashSet<int>>(() => []);
        foreach (var supported in allSupported)
        {
            foreach (var i in destroyed)
            {
                if (supportedBy[supported].Contains(i)) 
                {
                    toAddBack[supported].Add(i);
                    supportedBy[supported].Remove(i);
                }
            }
        }

        var result = destroyed.Count 
            + FallingBrickCount(allSupported.Where(j => supportedBy[j].Count == 0).ToHashSet(), supports, supportedBy);

        foreach (var (supported, toAdd) in toAddBack)
        {
            foreach (var i in toAdd)
            {
                supportedBy[supported].Add(i);       
            }
        }
        
        return result;
    }

    private static IEnumerable<int> BricksBelow(Layer layer, Brick brick)
    {
        return brick.Start.X.EnumerateTo(brick.End.X, inclusive: true)
            .Pair(brick.Start.Y.EnumerateTo(brick.End.Y, inclusive: true))
            .Select(pair => new Point(pair.Item1, pair.Item2))
            .Select(point => layer[point])
            .Where(b => b is not null)
            .Cast<int>()
            .Distinct();
    }
}

internal class Brick(Point3D start, Point3D end)
{
    public Point3D Start { get; set; } = start;
    public Point3D End { get; set; } = end;

    internal static Brick Parse(string s)
    {
        (var start, var end) = s.Split('~')
            .Select(Point3D.Parse)
            .OrderBy(p => p.X).ThenBy(p => p.Y).ThenBy(p => p.Z)
            .AsTuple2();
        return new Brick(start, end);
    }
}

internal class Point3D(int x, int y, int z)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Z { get; set; } = z;

    internal static Point3D Parse(string s)
    {
        (var x, var y, var z) = s.Split(',').Select(int.Parse).AsTuple3();
        return new Point3D(x, y, z);
    }
}

internal class Layer : SparseGrid<int?>
{
    public Layer() : base(null)
    {
    }
}