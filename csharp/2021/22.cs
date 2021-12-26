using System.Text.RegularExpressions;

public class Solver202122 : ISolver
{
    private static readonly Regex inputRegex = new Regex(@"(on|off) x=(.+),y=(.+),z=(.+)", RegexOptions.Compiled);

    public dynamic Solve(string[] lines)
    {
        var instructions = lines.Select(ParseInstruction);
        var simplerInstructions = instructions.Where(IsSimplerInstruction);
        return (Reboot(simplerInstructions).Sum(cuboid => cuboid.Count),
            Reboot(instructions).Sum(cuboid => cuboid.Count));
    }

    private List<Cuboid> Reboot(IEnumerable<(bool IsOn, Cuboid Cuboid)> instructions)
    {
        var onCuboids = new List<Cuboid>();
        foreach (var instruction in instructions)
        {
            if (instruction.IsOn)
            {
                onCuboids.AddRange(Overlap(onCuboids, instruction.Cuboid));
                onCuboids = onCuboids.ToList();
            }
            else
            {
                onCuboids = onCuboids.SelectMany(cuboid => Split(cuboid, instruction.Cuboid)).ToList();
            }
        }
        return onCuboids;
    }

    private IEnumerable<Cuboid> Overlap(IEnumerable<Cuboid> onCuboids, Cuboid cuboid)
    {
        IEnumerable<Cuboid> newOnCuboids = new List<Cuboid>() { cuboid };
        foreach (var onCuboid in onCuboids)
        {
            newOnCuboids = Overlap(onCuboid, newOnCuboids);
        }
        return newOnCuboids;
    }

    private IEnumerable<Cuboid> Split(Cuboid onCuboid, Cuboid cuboid)
    {
        return Overlap(cuboid, onCuboid);
    }

    private IEnumerable<Cuboid> Overlap(Cuboid onCuboid, IEnumerable<Cuboid> toOverlap)
    {
        return toOverlap.SelectMany(cuboid => Overlap(onCuboid, cuboid));
    }

    private IEnumerable<Cuboid> Overlap(Cuboid onCuboid, Cuboid toOverlap)
    {
        var cuboids = onCuboid.Axes.Zip(toOverlap.Axes)
            .Select(item => Overlap(item.First, item.Second))
            .CombineAll()
            .Where(axes => !axes.All(axis => axis.Overlaps))
            .Select(axes => new Cuboid(axes.Select(axis => (axis.Min, axis.Max)))).ToArray();
        return Merge(Merge(cuboids));
    }

    private IEnumerable<Cuboid> Merge(IEnumerable<Cuboid> cuboids)
    {
        Cuboid? prevCuboid = null;
        foreach (var cuboid in cuboids)
        {
            var mergedCuboid = Merge(cuboid, prevCuboid);
            if (mergedCuboid is not null)
            {
                prevCuboid = mergedCuboid;
            }
            else
            {
                yield return prevCuboid!.Value;
                prevCuboid = cuboid;
            }
        }
        if (prevCuboid is not null)
        {
            yield return prevCuboid.Value;
        }
    }

    private Cuboid? Merge(Cuboid cuboid, Cuboid? otherCuboid)
    {
        if (otherCuboid is null)
        {
            return cuboid;
        }
        List<(int Min, int Max)> axes = new List<(int Min, int Max)>();
        bool foundTouchingAxis = false;
        foreach (var (axis1, axis2) in cuboid.Axes.Zip(otherCuboid.Value.Axes))
        {
            if (axis1 == axis2)
            {
                axes.Add(axis1);
            }
            else if (axis1.Max + 1 == axis2.Min)
            {
                if (foundTouchingAxis) {
                    return null;
                }
                axes.Add((axis1.Min, axis2.Max));
                foundTouchingAxis = true;
            }
            else if (axis2.Max + 1 == axis1.Min)
            {
                if (foundTouchingAxis) {
                    return null;
                }
                axes.Add((axis2.Min, axis1.Max));
                foundTouchingAxis = true;
            }
            else
            {
                return null;
            }
        }
        return new Cuboid(axes);
    }

    private IEnumerable<(int Min, int Max, bool Overlaps)> Overlap((int Min, int Max) onAxis, (int Min, int Max) toOverlap)
    {
        if (onAxis.Min > toOverlap.Min)
        {
            if (onAxis.Min > toOverlap.Max)
            {
                yield return (toOverlap.Min, toOverlap.Max, false);
            }
            else
            {
                yield return (toOverlap.Min, onAxis.Min - 1, false);
                if (onAxis.Max >= toOverlap.Max)
                {
                    yield return (onAxis.Min, toOverlap.Max, true);
                }
                else
                {
                    yield return (onAxis.Min, onAxis.Max, true);
                    yield return (onAxis.Max + 1, toOverlap.Max, false);
                }
            }
        }
        else
        {
            if (toOverlap.Min > onAxis.Max)
            {
                yield return (toOverlap.Min, toOverlap.Max, false);
            }
            else
            {
                if (onAxis.Max >= toOverlap.Max)
                {
                    yield return (toOverlap.Min, toOverlap.Max, true);
                }
                else 
                {
                    yield return (toOverlap.Min, onAxis.Max, true);
                    yield return (onAxis.Max + 1, toOverlap.Max, false);
                }
            }
        }
    }

    private bool IsSimplerInstruction((bool IsOn, Cuboid Cuboid) instruction)
    {
        return instruction.Cuboid.Axes.All(axis => axis.Min >= -50 && axis.Max <= 50);
    }

    private (bool IsOn, Cuboid Cuboid) ParseInstruction(string s)
    {
        var m = inputRegex.Match(s);
        return (m.Groups[1].Value == "on",
            new Cuboid(m.Groups.Values.Skip(2).Select(group => group.Value).Select(ParseAxes)));
    }

    private (int Min, int Max) ParseAxes(string s)
    {
        var values = s.Split("..").Select(int.Parse);
        return (values.Min(), values.Max());
    }
}

struct Cuboid
{
    public IEnumerable<(int Min, int Max)> Axes { get; }

    public long Count
    {
        get
        {
            return Axes.Select(axis => axis.Max - axis.Min + 1L).Product();
        }
    }

    public Cuboid(IEnumerable<(int Min, int Max)> axes)
    {
        this.Axes = axes.Select(axis => (axis.Min, axis.Max));
    }

}