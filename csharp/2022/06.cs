using Aoc;

namespace Aoc2022;

public class Solver202206 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        return (
            FindStartMarker(lines[0], 4),
            FindStartMarker(lines[0], 14)
        );
    }

    private static int FindStartMarker(string signal, int window)
    {
        var uniqueChars = new Counter<char>(signal.Take(window));
        var i = 0;
        while (uniqueChars.Count != window)
        {
            uniqueChars.Remove(signal[i]);
            uniqueChars.Add(signal[i + window]);
            i++;
        }
        return i + window;
    }
}