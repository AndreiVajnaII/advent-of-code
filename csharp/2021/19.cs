using System.Collections.Immutable;

public class Solver202119 : ISolver
{
    private IEnumerable<Transform> Transforms = GenerateTransforms(3);

    public dynamic Solve(string[] lines)
    {
        var scanners = Helpers.GroupLines(lines).Select(ParseScanner);
        var originScanner = scanners.First();
        scanners = scanners.Skip(1);
        while (scanners.Count() > 0)
        {
            scanners = Merge(originScanner, scanners).ToArray();
        }
        return (originScanner.Beacons.Count,
            originScanner.ScannerCoords.Pairwise().Select(p => p.Item1.ManhattanDistanceTo(p.Item2)).Max());
    }

    private static IEnumerable<Transform> GenerateTransforms(int axes)
    {
        var indices = Enumerable.Range(0, axes).ToImmutableArray();
        var result = new List<Transform>();
        GenerateTransforms(indices, ImmutableArray<(int, int)>.Empty, result);
        return result;
    }

    private static void GenerateTransforms(
        ImmutableArray<int> remaining,
        ImmutableArray<(int, int)> current,
        List<Transform> result)
    {
        if (remaining.Length == 0)
        {
            result.Add(new Transform(current));
        }
        for (int i = 0; i < remaining.Length; i++)
        {
            GenerateTransforms(remaining.RemoveAt(i), current.Add((remaining[i], 1)), result);
            GenerateTransforms(remaining.RemoveAt(i), current.Add((remaining[i], -1)), result);
        }
    }

    private IEnumerable<Scanner> Merge(Scanner originScanner, IEnumerable<Scanner> scanners)
    {
        foreach (var scanner in scanners)
        {
            if (scanner.Distances.IntersectBy(originScanner.Distances.Select(d => d.Distance), d => d.Distance).Count() >= 66)
            {
                Merge(originScanner, scanner);
            }
            else
            {
                yield return scanner;
            }
        }
    }

    private void Merge(Scanner originScanner, Scanner scanner)
    {
        var (transformedScanner, scannerBeacon) = TransformScanner(originScanner, scanner);
        originScanner.AddBeacons(transformedScanner.Beacons.Except(originScanner.Beacons));
        originScanner.ScannerCoords.Add(scannerBeacon);
    }

    private (Scanner, Beacon) TransformScanner(Scanner originScanner, Scanner scanner)
    {
        var matchingDistances1 = originScanner.Distances.IntersectBy(scanner.Distances.Select(d => d.Distance), d => d.Distance).OrderBy(d => d.Distance);
        var originBeacons = matchingDistances1.SelectMany(d => new[] { d.B1, d.B2 }).Distinct();
        var matchingDistances2 = scanner.Distances.IntersectBy(originScanner.Distances.Select(d => d.Distance), d => d.Distance).OrderBy(d => d.Distance);
        var beacons = matchingDistances2.SelectMany(d => new[] { d.B1, d.B2 }).Distinct();
        foreach (var originBeacon in originBeacons)
        {
            foreach (var beacon in beacons)
            {
                foreach (var transform in Transforms)
                {
                    var translation = GetTranslation(originBeacon, Transform(beacon, transform));
                    var translatedScanner = Translate(Transform(scanner, transform), translation);
                    int count = originScanner.Beacons.Intersect(translatedScanner.Beacons).Count();
                    if (count >= 12)
                    {
                        return (translatedScanner, new Beacon(translation));
                    }
                }
            }
        }
        throw new InvalidOperationException("Should only be called for scanners that can be merged");
    }

    private Scanner Transform(Scanner scanner, Transform transform)
    {
        return new Scanner(scanner.Beacons.Select(beacon => Transform(beacon, transform)));
    }

    private Beacon Transform(Beacon beacon, Transform transform)
    {
        return new Beacon(transform.Apply(beacon.Coords));
    }

    private Scanner Translate(Scanner scanner, ImmutableArray<int> translation)
    {
        return new Scanner(scanner.Beacons.Select(beacon => Translate(beacon, translation)));
    }

    private Beacon Translate(Beacon beacon, ImmutableArray<int> translation)
    {
        return new Beacon(beacon.Coords.Zip(translation).Select(c => c.First + c.Second)
            .ToImmutableArray());
    }

    private ImmutableArray<int> GetTranslation(Beacon originBeacon, Beacon beacon)
    {
        return originBeacon.Coords.Zip(beacon.Coords).Select(c => c.First - c.Second)
            .ToImmutableArray();
    }

    private Scanner ParseScanner(IEnumerable<string> lines)
    {
        return new Scanner(lines.Skip(1).Select(line => new Beacon(
            line.Split(",").Select(int.Parse).ToImmutableArray())));
    }
}

class Scanner
{
    public List<Beacon> Beacons { get; private set; }
    public List<Beacon> ScannerCoords { get; private set; } = new();
    public List<(Beacon B1, Beacon B2, int Distance)> Distances { get; private set; } = new();

    public Scanner(IEnumerable<Beacon> beacons)
    {
        Beacons = beacons.ToList();
        Distances.AddRange(CalculateDistances(beacons.Pairwise()));
    }

    public void AddBeacons(IEnumerable<Beacon> beacons)
    {
        Distances.AddRange(CalculateDistances(beacons.Pairwise()));
        Distances.AddRange(CalculateDistances(beacons.Pair(Beacons)));
        Beacons.AddRange(beacons);
    }

    private IEnumerable<(Beacon, Beacon, int)> CalculateDistances(IEnumerable<(Beacon, Beacon)> pairs)
    {
        return pairs.Select(p => (p.Item1, p.Item2, p.Item1.DistanceTo(p.Item2)));
    }
}

class Beacon
{
    public ImmutableArray<int> Coords { get; private set; }

    public Beacon(ImmutableArray<int> coords)
    {
        Coords = coords;
    }

    public override bool Equals(object? obj)
    {
        return obj is Beacon beacon &&
               Coords.SequenceEqual(beacon.Coords);
    }

    public override int GetHashCode()
    {
        return Coords.Aggregate(HashCode.Combine);
    }

    public int DistanceTo(Beacon other)
    {
        return Coords.Zip(other.Coords).Select(p => p.First - p.Second).Select(x => x * x).Sum();
    }

    public int ManhattanDistanceTo(Beacon other)
    {
        return Coords.Zip(other.Coords).Select(p => Math.Abs(p.First - p.Second)).Sum();
    }
}

class Transform
{
    private ImmutableArray<(int Index, int Sign)> indices;

    public Transform(ImmutableArray<(int, int)> indices)
    {
        this.indices = indices;
    }

    public ImmutableArray<int> Apply(ImmutableArray<int> coords)
    {
        return indices.Select(i => coords[i.Index] * i.Sign).ToImmutableArray();
    }
}