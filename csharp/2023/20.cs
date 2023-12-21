using Aoc;
using static Aoc.Helpers;
using static Aoc.NumberExtensions;

namespace Aoc2023;

public class Solver202320 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var modules = new Dictionary<string, CommModule>();
        var parsedLines = lines.Select(ParseLine).ToArray();
        foreach ((var name, var outputs) in parsedLines)
        {
            if (name == "broadcaster")
            {
                modules[name] = new Broadcaster(outputs);
            }
            else if (name[0] == '%')
            {
                modules[name[1..]] = new FlipFlop(outputs);
            }
            else
            {
                var inputs = parsedLines
                    .Where(line => line.Outputs.Contains(name[1..]))
                    .Select(line => line.Name == "broadcaster" ? line.Name : line.Name[1..])
                    .ToArray();
                modules[name[1..]] = new Conjunction(outputs, inputs);
            }
        }

        (var lowPulses, var highPulses) = 1000.Times(() => PushButton(modules))
            .Aggregate((result, tuple) =>
                (result.LowPulses + tuple.LowPulses, result.HighPulses + tuple.HighPulses));

        foreach (var module in modules.Values)
        {
            module.Reset();
        }

        var buttonPushes = 1.EnumerateTo(int.MaxValue)
            .Do(_ => PushButton(modules))
            .First(_ => (modules["gh"] as Conjunction).inputs["tv"] == false);

        // for part 2, uptate the modules so that they detect cycles in their state

        return (lowPulses * highPulses, buttonPushes);
    }

    private static (long LowPulses, long HighPulses) PushButton(Dictionary<string, CommModule> modules)
    {
        long lowPulses = 0;
        long highPulses = 0;
        var queue = new Queue<(bool Pulse, string Desination, string Source)>();
        queue.Enqueue((false, "broadcaster", "button"));
        while (queue.Count > 0)
        {
            (var pulse, var destination, var source) = queue.Dequeue();
            if (pulse == false) lowPulses++; else highPulses++;
            if (modules.TryGetValue(destination, out var module))
            {
                foreach (var (newPulse, newDestination) in module.Send(pulse, source))
                {
                    queue.Enqueue((newPulse, newDestination, destination));
                }
            }
            else 
            {
                if (pulse == false) {
                    modules.Add(destination, new Output());
                }
            }
        }
        return (lowPulses, highPulses);
    }

    private static (string Name, string[] Outputs) ParseLine(string line)
        => line.Split(" -> ").AsTuple2(Id, outputs => outputs.Split(", "));
}

internal abstract class CommModule
{
    public abstract IEnumerable<(bool Pulse, string Destination)> Send(bool pulse, string source);

    public virtual void Reset() { }
}

internal class Broadcaster(string[] outputs) : CommModule
{
    public override IEnumerable<(bool Pulse, string Destination)> Send(bool pulse, string source)
        => outputs.Select(output => (pulse, output));
}

internal class FlipFlop(string[] outputs) : CommModule
{
    private bool state = false;

    public override IEnumerable<(bool Pulse, string Destination)> Send(bool pulse, string source)
    {
        if (pulse == false)
        {
            state = !state;
            return outputs.Select(output => (state, output));
        }
        return Enumerable.Empty<(bool, string)>();
    }
}

internal class Conjunction(string[] outputs, string[] inputs) : CommModule
{
    public readonly Dictionary<string, bool> inputs = inputs.Select(input => (input, false)).ToDictionary();

    public override IEnumerable<(bool Pulse, string Destination)> Send(bool pulse, string source)
    {
        inputs[source] = pulse;
        return outputs.Select(desination => (!inputs.Values.All(value => value == true), desination));
    }

    public override void Reset()
    {
        foreach (var key in inputs.Keys)
        {
            inputs[key] = false;
        }
    }
}

internal class Output : CommModule
{
    public override IEnumerable<(bool Pulse, string Destination)> Send(bool pulse, string source)
        => Enumerable.Empty<(bool, string)>();
}
