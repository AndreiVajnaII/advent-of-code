using System.Text.RegularExpressions;
using Aoc;
using static Aoc.Helpers;

namespace Aoc2023;

public class Solver202301 : ISolver
{
    private readonly static string SimpleDigitPattern = @"\d";
    private readonly static string RealDigitPattern = @"one|two|three|four|five|six|seven|eight|nine|\d";

    public dynamic Solve(string[] lines)
    {
        return (
            lines.Select(CalibrationTotal(SimpleDigitPattern)).Sum(),
            lines.Select(CalibrationTotal(RealDigitPattern)).Sum()
        );
    }

    private Func<string, int> CalibrationTotal(string pattern)
        => line => FirstDigit(line, pattern) * 10 + LastDigit(line, pattern);

    private int FirstDigit(string line, string pattern)
    {
        var regex = new Regex(pattern);
        var match = regex.Match(line);
        return ParseDigit(match.Value);
    }

    private int LastDigit(string line, string pattern)
    {
        var regex = new Regex(pattern, RegexOptions.RightToLeft);
        var match = regex.Match(line);
        return ParseDigit(match.Value);
    }

    private static int ParseDigit(string digit) => digit switch
    {
            "one" => 1,
            "two" => 2,
            "three" => 3,
            "four" => 4,
            "five" => 5,
            "six" => 6,
            "seven" => 7,
            "eight" => 8,
            "nine" => 9,
            _ when Char.IsDigit(digit[0]) => ParseChar(digit[0]),
            _ => throw new ArgumentException("Invalid digit: " + digit)
    };
}
