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
             select PriorityOfType(BadgeOfElfGroup(elfGroup.ToList())))
                .Sum()
        );
    }

    private char BadgeOfElfGroup(List<string> elves)
    {
        return elves.Select(Enumerable.AsEnumerable).Aggregate(Enumerable.Intersect).Single();
    }

    private char MisplacedType(string rucksack)
    {
        return rucksack.Take(rucksack.Length / 2).Intersect(rucksack.Skip(rucksack.Length / 2)).Single();
    }

    private int PriorityOfType(char type)
    {
        if ('a' <= type && type <= 'z')
        {
            return type - 'a' + 1;
        }
        else
        {
            return type - 'A' + 27;
        }
    }
}