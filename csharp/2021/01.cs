using Aoc;

namespace Aoc2021;

public class Solver202101: ISolver {
    public dynamic Solve(string[] lines) {
        var depths = lines.Select(s => Int64.Parse(s));
        return (
            Count(depths),
            Count(depths.Zip(depths.Skip(1), depths.Skip(2))
                .Select(t => t.First + t.Second + t.Third)));
    }

    private int Count(IEnumerable<long> depths) {
        return depths.Zip(depths.Skip(1)).Count(t => t.Second > t.First);
    }
}
