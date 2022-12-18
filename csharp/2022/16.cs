using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Aoc;

namespace Aoc2022;

public class Solver202216 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var valves = lines.Select(Valve.Parse).ToDictionary(valve => valve.Name);

        foreach (var valveK in valves.Values)
        {
            foreach (var valveI in valves.Values)
            {
                foreach (var valveJ in valves.Values)
                {
                    if (valveI.Distances.ContainsKey(valveK.Name) && valveK.Distances.ContainsKey(valveJ.Name))
                    {
                        var altDistance = valveI.Distances[valveK.Name] + valveK.Distances[valveJ.Name];
                        if (!valveI.Distances.ContainsKey(valveJ.Name) || altDistance < valveI.Distances[valveJ.Name])
                        {
                            valveI.Distances[valveJ.Name] = altDistance;
                        }
                    }
                }
            }
        }

        var states = new PriorityQueue<TraversalState, int>();


        states.Enqueue(new TraversalState(valves["AA"], 30, 0, ImmutableHashSet.Create<string>()), 0);
        var maxPressure = 0;
        while (states.Count > 0)
        {
            var state = states.Dequeue();
            maxPressure = Math.Max(maxPressure, state.Pressure);
            foreach (var newState in state.GetNewStates(valves))
            {
                states.Enqueue(newState, -newState.Pressure);
            }
        }

        return maxPressure;
    }

    private class Valve
    {
        private static readonly Regex Regex = new(@"Valve (.+) has flow rate=(\d+); tunnels? leads? to valves? (.+)");

        public string Name { get; }
        public int FlowRate { get; }
        public ImmutableHashSet<string> Tunnels { get; }

        public IDictionary<string, int> Distances = new Dictionary<string, int>();

        public Valve(string name, int flowRate, IEnumerable<string> tunnels)
        {
            Name = name;
            FlowRate = flowRate;
            Tunnels = tunnels.ToImmutableHashSet();
            foreach (var tunnel in Tunnels)
            {
                Distances[tunnel] = 1;
            }
        }

        public static Valve Parse(string line)
        {
            var groups = Regex.Match(line).GroupValues().ToArray();
            return new Valve(groups[0], int.Parse(groups[1]),
                groups[2].Split(", "));
        }
    }

    private class TraversalState
    {
        public Valve CurrentValve { get; }
        public int RemainingMinutes { get; set; }
        public int Pressure { get; set; }
        public IImmutableSet<string> OpenValves;

        public TraversalState(Valve valve, int minutes, int pressure, IImmutableSet<string> openValves)
        {
            CurrentValve = valve;
            RemainingMinutes = minutes;
            Pressure = pressure;
            OpenValves = openValves;
        }

        public IEnumerable<TraversalState> GetNewStates(Dictionary<string, Valve> valves)
        {
            var newOpenValves = OpenValves.Add(CurrentValve.Name);
            var remainingValves = valves.Values.Where(valve => !newOpenValves.Contains(valve.Name))
                .Where(valve => valve.FlowRate > 0);
            foreach (var valve in remainingValves)
            {
                var remainingMinutes = RemainingMinutes - CurrentValve.Distances[valve.Name] - 1;
                if (remainingMinutes > 0)
                {
                    yield return new TraversalState(valve, remainingMinutes,
                        Pressure + remainingMinutes * valve.FlowRate, newOpenValves);
                }
            }
        }
    }
}