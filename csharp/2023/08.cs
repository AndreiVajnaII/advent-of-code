using System.Data;
using Aoc;
using static Aoc.Helpers;
using static Aoc.Graph;

namespace Aoc2023;

public class Solver202308 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var instructions = new CircularEnumerator<char>(lines[0]);
        var network = lines.Skip(2).Select(ParseLink).ToDictionary();

        return (
            CountSteps("AAA", node => node == "ZZZ", network, instructions),
            network.Keys.Where(node => node.EndsWith('A'))
                .Select(start => CountSteps(start, node => node.EndsWith('Z'), network, instructions))
                .Aggregate(LeastCommonMultiple)
        );
    }

    private static long CountSteps(string start, Func<string, bool> hasReachedEnd,
        Dictionary<string, (string, string)> network, CircularEnumerator<char> instructions)
    {
        var steps = Walk(start, hasReachedEnd, node => SelectLeftRight(network[node], instructions.Next()))
            .LongCount();
        instructions.Reset();
        return steps;
    }

    private static string SelectLeftRight((string Left, string Right) leftRight, char instruction)
    {
        return instruction == 'L' ? leftRight.Left : leftRight.Right;
    }

    private (string Node, (string Left, string Right)) ParseLink(string line)
    {
        return line.Split(" = ").AsTuple2(s => s, s => s[1..^1].Split(", ").AsTuple2());
    }
}
