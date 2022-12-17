using System.Text.RegularExpressions;
using Aoc;
using static Aoc.TupleHelpers;

namespace Aoc2022;

public class Solver202215 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        const int yOfInterest = 2000000;
        const int maxY = 4000000;
        var sensors = lines.Select(Sensor.Parse).OrderBy(sensor => sensor.Location.X).ToArray();
        var beacons = sensors.Select(sensor => sensor.ClosestBeacon)
            .Distinct()
            .Where(beacon => beacon.Y == yOfInterest)
            .ToArray();

        return (
            ExclusionsForLine(yOfInterest, sensors)
                .Select(exclusion => exclusion.End - exclusion.Start + 1)
                .Sum() - beacons.Length,
            Enumerable.Range(0, maxY).Select(y => (y, ExclusionsForLine(y, sensors)))
                .Where(((int Y, List<Interval> Exclusions) tuple) => tuple.Exclusions.Count == 2)
                .Select(((int Y, List<Interval> Exclusions) tuple) => (
                    tuple.Y,
                    Math.Min(tuple.Exclusions[0].End, tuple.Exclusions[1].End) + 1))
                .Select(((int Y, int X) tuple) => tuple.X * (long)maxY + tuple.Y)
                .First()
        );
    }

    private static List<Interval> ExclusionsForLine(int y, IEnumerable<Sensor> sensors)
    {
        var exclusions = sensors.Select(sensor => sensor.ExclusionForLine(y))
            .Where(exclusion => exclusion is not null).Cast<Interval>()
            .ToList();
        bool done;
        do
        {
            done = true;
            var newExclusions = exclusions.Aggregate(new List<Interval>(), Union);
            if (newExclusions.Count < exclusions.Count)
            {
                exclusions = newExclusions;
                done = false;
            }
        } while (!done);

        return exclusions;
    }

    private static List<Interval> Union(List<Interval> result, Interval interval)
    {
        var merged = false;
        foreach (var prev in result)
        {
            if (IsOverlapping((prev.Start, prev.End), (interval.Start, interval.End)))
            {
                prev.Start = Math.Min(prev.Start, interval.Start);
                prev.End = Math.Max(prev.End, interval.End);
                merged = true;
            }
        }

        if (!merged)
        {
            result.Add(interval);
        }

        return result;
    }

    private class Sensor
    {
        private static readonly Regex PointRegex = new(@"(-?\d+)");

        public Point Location { get; }
        public Point ClosestBeacon { get; }
        public int Radius => Location.ManhattanDistance(ClosestBeacon);

        public Sensor(Point location, Point closestBeacon)
        {
            Location = location;
            ClosestBeacon = closestBeacon;
        }

        public static Sensor Parse(string line)
        {
            var numbers = PointRegex.Matches(line).Select(match => match.Value)
                .Select(int.Parse).ToArray();
            return new Sensor(
                new Point(numbers[0], numbers[1]),
                new Point(numbers[2], numbers[3]));
        }

        public Interval? ExclusionForLine(int y)
        {
            var lineRadius = Radius - Math.Abs(y - Location.Y);
            return lineRadius < 0 ? null : new Interval(Location.X - lineRadius, Location.X + lineRadius);
        }
    }

    private class Interval
    {
        public int Start { get; set; }
        public int End { get; set; }

        public Interval(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}