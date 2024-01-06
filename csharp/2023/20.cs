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
            .Select(signals => signals.Aggregate((Low: 0, High: 0),
                (result, signal) => signal.Pulse == false
                    ? (result.Low + 1, result.High)
                    : (result.Low, result.High + 1)))
            .Aggregate((result, tuple) =>
                (result.Low + tuple.Low, result.High + tuple.High));

        var outputFeeder = modules.Single(entry => entry.Value is Conjunction c && c.Outputs.Contains("rx"));
        var buttonPushes = (outputFeeder.Value as Conjunction)!.Inputs.Keys
            .Select(inputName => FindFirstActivation(outputFeeder.Key, inputName, modules)).ToArray();

        return (
            lowPulses * highPulses,
            buttonPushes.Aggregate(LeastCommonMultiple)
        );
    }

    private static IEnumerable<(bool Pulse, string Destination, string Source)> PushButton(Dictionary<string, CommModule> modules)
    {
        var queue = new Queue<(bool Pulse, string Desination, string Source)>();
        queue.Enqueue((false, "broadcaster", "button"));
        while (queue.Count > 0)
        {
            var signal = queue.Dequeue();
            yield return signal;
            (var pulse, var destination, var source) = signal;
            if (modules.TryGetValue(destination, out var module))
            {
                foreach (var (newPulse, newDestination) in module.Send(pulse, source))
                {
                    queue.Enqueue((newPulse, newDestination, destination));
                }
            }
        }
    }

    private static long FindFirstActivation(string destination, string input, Dictionary<string, CommModule> modules)
    {
        ResetModules(modules);
        return 1.EnumerateTo(int.MaxValue)
            .First(_ => PushButton(modules).Contains((true, destination, input)));
    }

    private static (string Name, string[] Outputs) ParseLine(string line)
        => line.Split(" -> ").AsTuple2(Id, outputs => outputs.Split(", "));

    private static void ResetModules(Dictionary<string, CommModule> modules)
    {
        foreach (var module in modules.Values)
        {
            module.Reset();
        }
    }
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

    public override void Reset()
    {
        state = false;
    }
}

internal class Conjunction(string[] outputs, string[] inputs) : CommModule
{
    public string[] Outputs { get; private set; } = outputs;
    public readonly Dictionary<string, bool> Inputs = inputs.Select(input => (input, false)).ToDictionary();

    public override IEnumerable<(bool Pulse, string Destination)> Send(bool pulse, string source)
    {
        Inputs[source] = pulse;
        return Outputs.Select(destination => (!Inputs.Values.All(value => value == true), destination));
    }

    public override void Reset()
    {
        foreach (var key in Inputs.Keys)
        {
            Inputs[key] = false;
        }
    }
}
