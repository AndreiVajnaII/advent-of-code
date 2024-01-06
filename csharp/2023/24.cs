using Aoc;

namespace Aoc2023;

public class Solver202324 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        const long min = 200000000000000;
        const long max = 400000000000000;
        // const double min = 7;
        // const double max = 27;
        var hailstones = lines.Select(Hailstone.Parse).ToArray();

        return (
            hailstones
                .Pairwise()
                .Select(pair => Hailstone.IntersectXY(pair.Item1, pair.Item2))
                .Where(point => point.X >= min && point.X <= max
                    && point.Y >= min && point.Y <= max)
                .Count(),
                hailstones.Pairwise().Min(pair => Math.Abs(pair.Item1.Start.X - pair.Item2.Start.X))
            // GetStartCoordinate(hailstones.Select(stone => (stone.Start.Z, stone.Velocity.dZ)))
            // + GetStartCoordinate(hailstones.Select(stone => (stone.Start.Y, stone.Velocity.dY)))
            // + GetStartCoordinate(hailstones.Select(stone => (stone.Start.Z, stone.Velocity.dZ)))
        );
    }

    private long GetStartCoordinate(IEnumerable<(long Coord, long Velocity)> hailstones)
    {
        var ordered = hailstones.OrderBy(stone => stone.Coord).ThenBy(stone => stone.Velocity).ToArray();
        var minVelocity = ordered.Min(stone => stone.Velocity);
        var maxVelocity = ordered.Max(stone => stone.Velocity);
        var maxDistance = ordered[^1].Coord - ordered[0].Coord;

        for (long c = ordered[^1].Coord + 1; c <= ordered[^1].Coord + maxDistance; c++)
        {
            var possibleSpeeds = GetDivisors(c - ordered[^1].Coord)
                .Select(d => ordered[^1].Velocity - d)
                .Where(v => v < minVelocity)
                .ToArray();
            for (int i = ordered.Length - 2; i >= 0; i--)
            {
                possibleSpeeds = possibleSpeeds.Where(speed => (ordered[i].Coord - c) % (speed - ordered[i].Velocity) == 0).ToArray();
                if (possibleSpeeds.Length == 0)
                {
                    break;
                }
            }
            if (possibleSpeeds.Length > 0) return c;
        }

        for (long c = ordered[0].Coord - 1; c >= ordered[0].Coord - maxDistance; c--)
        {
            var possibleSpeeds = GetDivisors(ordered[0].Coord - c)
                .Select(d => ordered[0].Velocity + d)
                .Where(v => v > maxVelocity)
                .ToArray();
            for (int i = 1; i < ordered.Length; i++)
            {
                possibleSpeeds = possibleSpeeds.Where(speed => (ordered[i].Coord - c) % (speed - ordered[i].Velocity) == 0).ToArray();
                if (possibleSpeeds.Length == 0)
                {
                    break;
                }
            }
            if (possibleSpeeds.Length > 0) return c;
        }

        throw new InvalidOperationException("Coord not found!");
    }

    private IEnumerable<long> GetDivisors(long n)
    {
        var bounds = (long)Math.Floor(Math.Sqrt(n));
        for (int i = 1; i < bounds; i++)
        {
            if (n % i == 0)
            {
                yield return i;
                yield return n / i;
            }
        }
        if (n == bounds * bounds)
        {
            yield return bounds;
        }
    }

    private long GetStartCoordinate2(IEnumerable<(long Coord, long Velocity)> hailstones)
    {
        var ordered = hailstones.OrderBy(stone => stone.Coord).ThenBy(stone => stone.Velocity).ToArray();
        var minVelocity = ordered.Min(stone => stone.Velocity);
        var maxVelocity = ordered.Max(stone => stone.Velocity);
        var maxDistance = ordered[^1].Coord - ordered[0].Coord;

        var minT1 = ordered.Min(stone => stone.Coord + stone.Velocity);
        var maxT1 = ordered.Max(stone => stone.Coord + stone.Velocity);

        for (long c = ordered[0].Coord; c > ordered[0].Coord - maxDistance; c--)
        {
            for (long v = maxVelocity; c + v <= minT1; v++)
            {
                if (ordered.All(stone => stone.Velocity == v
                    || (stone.Coord - c) % (stone.Velocity - v) == 0))
                {
                    return c;
                }
            }
        }
        for (long c = ordered[^1].Coord; c < ordered[^1].Coord + maxDistance; c++)
        {
            for (long v = minVelocity; c + v >= maxT1; v--)
            {
                if (ordered.All(stone => stone.Velocity == v
                    || (stone.Coord - c) % (stone.Velocity - v) == 0))
                {
                    return c;
                }
            }
        }
        throw new InvalidOperationException("Coord not found!");
    }

}

internal class Hailstone(long px, long py, long pz, long vx, long vy, long vz)
{
    public (long X, long Y, long Z) Start { get; } = (px, py, pz);
    public (long dX, long dY, long dZ) Velocity { get; } = (vx, vy, vz);

    public double SlopeXY { get; } = vy / (double)vx;
    public double InterceptXY { get; } = py - vy / (double)vx * px;

    public static Hailstone Parse(string s)
    {
        var ((px, py, pz), (vx, vy, vz)) = s.Split(" @ ")
            .Select(part => part.Split(", ").Select(long.Parse).AsTuple3())
            .AsTuple2();
        return new Hailstone(px, py, pz, vx, vy, vz);
    }

    public static (double X, double Y) IntersectXY(Hailstone line1, Hailstone line2)
    {
        var x = -(line2.InterceptXY - line1.InterceptXY) / (line2.SlopeXY - line1.SlopeXY);
        var y = line1.SlopeXY * x + line1.InterceptXY;
        return Math.Sign(x - line1.Start.X) == Math.Sign(line1.Velocity.dX)
            && Math.Sign(x - line2.Start.X) == Math.Sign(line2.Velocity.dX)
            ? (x, y)
            : (0, 0);
    }
}

