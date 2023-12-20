using System.Collections.Immutable;
using Aoc;
using static Aoc.Helpers;

namespace Aoc2023;

using PartRange = ImmutableDictionary<string, (int Start, int End)>;

public class Solver202319 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        (var workflows, var parts) = GroupLines(lines).AsTuple2(
            group => group.Select(ParseWorkflow).ToDictionary(),
            group => group.Select(ParsePart)
        );

        var partRange = ImmutableDictionary.CreateRange(
            "xmas".Select(c => KeyValuePair.Create(c.ToString(), (1, 4000))));

        return (
            parts.Where(part => RunWorkflows(part, workflows) == "A")
                .Select(part => part.Values.Sum()).Sum(),
            RunWorfklows(partRange, workflows)
                .Select(ranges => ranges.Values
                    .Select(range => (long)range.End - range.Start + 1)
                    .Product())
                .Sum()
        );
    }

    private static string RunWorkflows(Dictionary<string, int> part, Dictionary<string, string[]> workflows)
    {
        var current = "in";
        while (current != "A" && current != "R")
        {
            current = RunWorkflow(part, workflows[current], workflows);
        }
        return current;
    }

    private static string RunWorkflow(Dictionary<string, int> part, string[] workflow, Dictionary<string, string[]> workflows)
    {
        foreach (var rule in workflow)
        {
            if (RunRule(part, rule, out var result)) return result;
        }
        return "";
    }

    private static bool RunRule(Dictionary<string, int> part, string rule, out string result)
    {
        if (rule.Contains(':'))
        {
            (var condition, result) = rule.Split(':').AsTuple2();
            if (condition.Contains('<'))
            {
                (var rating, var value) = condition.Split('<').AsTuple2(Id, int.Parse);
                return part[rating] < value;
            }
            else
            {
                (var rating, var value) = condition.Split('>').AsTuple2(Id, int.Parse);
                return part[rating] > value;
            }
        }
        result = rule;
        return true;
    }

    private static List<PartRange> RunWorfklows(PartRange partRange, Dictionary<string, string[]> workflows)
    {
        return RunWorkflows(partRange, "in", workflows);
    }

    private static List<PartRange> RunWorkflows(PartRange partRange, string current, Dictionary<string, string[]> workflows)
    {
        if (current == "R") return [];
        if (current == "A") return [partRange];

        var resultRanges = new List<PartRange>();
        foreach (var rule in workflows[current])
        {
            if (rule.Contains(':'))
            {
                (var condition, var output) = rule.Split(':').AsTuple2();
                if (condition.Contains('<'))
                {
                    (var rating, var value) = condition.Split('<').AsTuple2(Id, int.Parse);
                    if (partRange[rating].Start < value)
                    {
                        var newRange = (partRange[rating].Start, Math.Min(value - 1, partRange[rating].End));
                        var newPartRange = partRange.SetItem(rating, newRange);
                        resultRanges.AddRange(RunWorkflows(newPartRange, output, workflows));
                    }
                    if (partRange[rating].End >= value)
                    {
                        var newRange = (Math.Max(value, partRange[rating].Start), partRange[rating].End);
                        partRange = partRange.SetItem(rating, newRange);
                    }
                }
                else
                {
                    (var rating, var value) = condition.Split('>').AsTuple2(Id, int.Parse);
                    if (partRange[rating].End > value)
                    {
                        var newRange = (Math.Max(value + 1, partRange[rating].Start), partRange[rating].End);
                        var newPartRange = partRange.SetItem(rating, newRange);
                        resultRanges.AddRange(RunWorkflows(newPartRange, output, workflows));
                    }
                    if (partRange[rating].Start <= value)
                    {
                        var newRange = (partRange[rating].Start, Math.Min(value, partRange[rating].End));
                        partRange = partRange.SetItem(rating, newRange);
                    }
                }
            }
            else
            {
                resultRanges.AddRange(RunWorkflows(partRange, rule, workflows));
            }
        }
        return resultRanges;
    }

    public static (string Label, string[] Rules) ParseWorkflow(string line)
        => line.Split('{').AsTuple2(Id, s => s[0..^1].Split(',').ToArray());

    public static Dictionary<string, int> ParsePart(string line)
        => line[1..^1].Split(",").Select(rating => rating.Split("=")
            .AsTuple2(Id, int.Parse)).ToDictionary();
}
