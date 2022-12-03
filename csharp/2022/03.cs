using Aoc;

namespace Aoc2022;

public class Solver202203 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        return (
            lines.Select(MisplacedType).Select(PriorityOfType).Sum(),
            (from i in Enumerable.Range(0, lines.Length)
             group lines[i] by i / 3 into elfGroup
             select PriorityOfType(BadgeOfElfGroup(elfGroup)))
                .Sum()
        );
    }

    private static char BadgeOfElfGroup(IEnumerable<string> elves)
    {
        return elves.Select(Enumerable.AsEnumerable).Aggregate(Enumerable.Intersect).Single();
    }

    private static char MisplacedType(string rucksack)
    {
        return rucksack.Take(rucksack.Length / 2).Intersect(rucksack.Skip(rucksack.Length / 2)).Single();
    }

    private static int PriorityOfType(char type)
    {
        if (type is >= 'a' and <= 'z')
        {
            return type - 'a' + 1;
        }
        else
        {
            return type - 'A' + 27;
        }
    }
}