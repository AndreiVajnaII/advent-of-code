using Aoc;
using static Aoc.Helpers;

namespace Aoc2022;

public class Solver202201 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var elfCalories = GroupLines(lines).Select(elf => elf.Select(long.Parse).Sum())
            .ToArray();

        return (
            elfCalories.Max(),
            elfCalories.OrderByDescending(x => x).Take(3).Sum()
        );
    }
}