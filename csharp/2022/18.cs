using Aoc;
using static Aoc.Graph;

namespace Aoc2022;

public class Solver202218 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var cubes = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();
        foreach (var cube in lines.Select(ParseCube))
        {
            var coveredSides = 6;
            foreach (var neighbour in NeighboursOf(cube))
            {
                var neighbourValue = GetCube(cubes, neighbour);
                if (neighbourValue is null) continue;
                SetCube(cubes, neighbour, neighbourValue.Value - 1);
                coveredSides--;
            }

            SetCube(cubes, cube, coveredSides);
        }

        var startX = cubes.Keys.Min();
        var startY = cubes[startX].Keys.Min();
        var startZ = cubes[startX][startY].Keys.Min();
        var startSide = new CubeSide(
            new[] {startX - 1, startY, startZ},
            new[] {startX, startY, startZ}
        );

        var visitedSides = new HashSet<CubeSide>();
        foreach (var visitedSide in Traverse(
                     startSide,
                     currentSide => NeighbourSidesOf(currentSide, cubes)
                         .Where(side =>
                             GetCube(cubes, side.Outer) == null &&
                             GetCube(cubes, side.Inner) != null)
                         .Where(side => !visitedSides.Contains(side))))
        {
            visitedSides.Add(visitedSide);
        }


        return (
            cubes.Values
                .SelectMany(x => x.Values)
                .SelectMany(x => x.Values)
                .Sum(),
            visitedSides.Count
        );
    }

    private static int? GetCube(IReadOnlyDictionary<int, Dictionary<int, Dictionary<int, int>>> cubes, int[] location)
    {
        if (!cubes.ContainsKey(location[0])) return null;
        if (!cubes[location[0]].ContainsKey(location[1])) return null;
        if (!cubes[location[0]][location[1]].ContainsKey(location[2])) return null;
        return cubes[location[0]][location[1]][location[2]];
    }

    private static void SetCube(IDictionary<int, Dictionary<int, Dictionary<int, int>>> cubes, int[] location,
        int value)
    {
        if (!cubes.ContainsKey(location[0]))
            cubes[location[0]] = new Dictionary<int, Dictionary<int, int>>();
        if (!cubes[location[0]].ContainsKey(location[1]))
            cubes[location[0]][location[1]] = new Dictionary<int, int>();
        cubes[location[0]][location[1]][location[2]] = value;
    }

    private static IEnumerable<int[]> NeighboursOf(int[] cube)
    {
        var neighbours = new[]
        {
            new[] {1, 0, 0},
            new[] {-1, 0, 0},
            new[] {0, 1, 0},
            new[] {0, -1, 0},
            new[] {0, 0, 1},
            new[] {0, 0, -1}
        };
        return neighbours.Select(neighbour => cube.Zip(neighbour)
            .Select(tuple => tuple.First + tuple.Second).ToArray());
    }

    private int[] ParseCube(string line)
    {
        return line.Split(",").Select(int.Parse).ToArray();
    }

    private IEnumerable<CubeSide> NeighbourSidesOf(CubeSide side,
        IReadOnlyDictionary<int, Dictionary<int, Dictionary<int, int>>> cubes)
    {
        for (var i = 0; i < side.Outer.Length; i++)
        {
            if (i == side.DifferentAxis)
            {
                continue;
            }

            yield return NeighbourSidesOf(side, i, 1).First(neighbour =>
                GetCube(cubes, neighbour.Outer) == null &&
                GetCube(cubes, neighbour.Inner) != null);
            yield return NeighbourSidesOf(side, i, -1).First(neighbour =>
                GetCube(cubes, neighbour.Outer) == null &&
                GetCube(cubes, neighbour.Inner) != null);
        }
    }

    private IEnumerable<CubeSide> NeighbourSidesOf(CubeSide side, int axisToModify, int direction)
    {
        yield return new CubeSide(
            side.Outer,
            side.Outer.Select((v, axis) => axis == axisToModify ? v + direction : v).ToArray());
        yield return new CubeSide(
            side.Outer.Select((v, axis) => axis == axisToModify ? v + direction : v).ToArray(),
            side.Inner.Select((v, axis) => axis == axisToModify ? v + direction : v).ToArray());
        yield return new CubeSide(
            side.Inner.Select((v, axis) => axis == axisToModify ? v + direction : v).ToArray(),
            side.Inner);
    }

    private class CubeSide : IEquatable<CubeSide>
    {
        public int[] Outer { get; }
        public int[] Inner { get; }
        public int DifferentAxis { get; }

        public CubeSide(int[] outer, int[] inner)
        {
            Outer = outer;
            Inner = inner;
            DifferentAxis = Enumerable.Range(0, Outer.Length).Single(i => outer[i] != inner[i]);
        }

        public override string ToString()
        {
            return $"{string.Join(",", Outer)} | {string.Join(",", Inner)}";
        }

        public bool Equals(CubeSide? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Outer.Zip(other.Outer).All(tuple => tuple.First == tuple.Second)
                   && Inner.Zip(other.Inner).All(tuple => tuple.First == tuple.Second);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CubeSide) obj);
        }

        public override int GetHashCode()
        {
            return Outer.Concat(Inner).Aggregate(HashCode.Combine);
        }

        public static bool operator ==(CubeSide? left, CubeSide? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CubeSide? left, CubeSide? right)
        {
            return !Equals(left, right);
        }
    }
}