public class Solver202108 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var inputs = lines.Select(ParseLine);
        var outputValues = inputs.Select(Decode);
        return (
            outputValues.Sum(value => value.Count(digit => digit == "1" || digit == "4" || digit == "7" || digit == "8")),
            outputValues.Sum(value => int.Parse(String.Join("", value))));
    }

    static private Dictionary<string, string> digits = new()
    {
        ["0"] = "abcefg",
        ["1"] = "cf",
        ["2"] = "acdeg",
        ["3"] = "acdfg",
        ["4"] = "bcdf",
        ["5"] = "abdfg",
        ["6"] = "abdefg",
        ["7"] = "acf",
        ["8"] = "abcdefg",
        ["9"] = "abcdfg"
    };

    private IEnumerable<string> Decode((string[], string[]) input)
    {
        var (signalPatterns, outputValue) = input;
        var mappings = new Dictionary<char, char>();
        var one = signalPatterns.First(s => s.Length == 2);
        var four = signalPatterns.First(s => s.Length == 4);
        var seven = signalPatterns.First(s => s.Length == 3);
        var eight = signalPatterns.First(s => s.Length == 7);
        mappings['a'] = seven.Except(one).Single();
        mappings['g'] = "abcdefg".Single(segment =>
            CountAppearances(segment, signalPatterns) == 7
            && CountAppearances(segment, signalPatterns.Except(new string[] { one, four, seven })) == 7);
        mappings['e'] = eight.Except(four.Union(seven).Append(mappings['g'])).Single();
        var remaining = "abcdefg".Except(new char[] { mappings['a'], mappings['g'], mappings['e'] });
        mappings['b'] = remaining.First(segment => CountAppearances(segment, signalPatterns) == 6);
        mappings['c'] = remaining.First(segment => CountAppearances(segment, signalPatterns) == 8);
        mappings['d'] = remaining.First(segment => CountAppearances(segment, signalPatterns) == 7);
        mappings['f'] = remaining.First(segment => CountAppearances(segment, signalPatterns) == 9);

        return outputValue.Select(value => ToDigit(value.Select(c => FindMapping(c, mappings)).ToArray()));
    }

    private char FindMapping(char c, Dictionary<char, char> mappings)
    {
        return mappings.First(entry => entry.Value == c).Key;
    }

    private (string[] SignalPatterns, string[] OutputValue) ParseLine(string line)
    {
        return line.Split(" | ").Select(s => s.Split(' ')).AsTuple2();
    }

    private int CountAppearances(char segment, IEnumerable<string> signalPatterns)
    {
        return signalPatterns.Count(pattern => pattern.Contains(segment));
    }

    private string ToDigit(char[] value)
    {
        return digits.First(digit => digit.Value.Length == value.Length
            && digit.Value.All(segment => value.Contains(segment)))
            .Key;
    }
}