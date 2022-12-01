using Aoc;

namespace Aoc2021;

public class Solver202114 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var sequence = new Sequence(lines[0]);
        var rules = ParseRules(lines.Skip(2));
        return (ScoreAfter(10, sequence, rules), ScoreAfter(30, sequence, rules));
    }

    private object ScoreAfter(int times, Sequence sequence, Dictionary<string, string> rules)
    {
        times.TimesDo(() => sequence.Insert(rules));
        return sequence.Score();
    }

    private Dictionary<string, string> ParseRules(IEnumerable<string> lines)
    {
        var rules = new Dictionary<string, string>();
        foreach (var line in lines)
        {
            var (key, value) = line.Split(" -> ").AsTuple2();
            rules.Add(key, value);
        }
        return rules;
    }
}

class Sequence
{
    private DictionaryWithDefault<string, long> pairCounts = new(0);
    private DictionaryWithDefault<char, long> charCounts = new(0);

    public Sequence(string sequence)
    {
        for (int i = 0; i < sequence.Length - 1; i++)
        {
            var pair = sequence.Substring(i, 2);
            pairCounts[pair]++;
            charCounts[pair[0]]++;
        }
        charCounts[sequence.Last()]++;
    }

    public void Insert(Dictionary<string, string> rules)
    {
        var newPairCounts = new DictionaryWithDefault<string, long>(0);
        foreach (var pair in pairCounts.Keys)
        {
            var toInsert = rules[pair];
            newPairCounts[pair[0] + toInsert] += pairCounts[pair];
            newPairCounts[toInsert + pair[1]] += pairCounts[pair];
            charCounts[toInsert[0]] += pairCounts[pair];
        }
        pairCounts = newPairCounts;
    }

    public long Score()
    {
        return charCounts.Values.Max() - charCounts.Values.Min();
    }
}