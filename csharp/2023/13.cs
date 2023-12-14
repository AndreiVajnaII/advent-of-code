using Aoc;
using static Aoc.Helpers;

namespace Aoc2023;

public class Solver202313 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        return (
            GroupLines(lines).Select(mirror => Summarize(mirror, 0)).Sum(),
            GroupLines(lines).Select(mirror => Summarize(mirror, 1)).Sum()
        );
    }

    private int Summarize(IEnumerable<string> lines, int smudges)
    {
        var grid = new Grid2D<int>(lines.Select(Parse).ToArray2D());
        if (TryFindReflection(grid.GridValueEnumerable(), out var index, smudges))
        {
            return index * 100;
        } 
        TryFindReflection(grid.ColumnValueEnumerable(), out index, smudges);
        return index;
    }

    private IEnumerable<int> Parse(string line)
        => line.Select(c => c == '#' ? 1 : 0);

    private static bool TryFindReflection(IEnumerable<IEnumerable<int>> enumerable, out int resultIndex,
        int smudges = 0)
    {
        var list = enumerable.ToList();
        var result = Enumerable.Range(0, list.Count - 1)
            .FirstOrNull(index => MirrorSides(list, index).Flatten()
                .Select(pair => pair.First ^ pair.Second)
                .Sum() == smudges);

        if (result.HasValue)
        {
            resultIndex = result.Value + 1;
            return true;
        }
        resultIndex = 0;
        return false;   
    }

    private static IEnumerable<IEnumerable<(int First, int Second)>> MirrorSides(List<IEnumerable<int>> list, int index)
    {
        for (var i = 0; index - i >= 0 && index + i + 1 < list.Count; i++)
        {
            yield return list[index - i].Zip(list[index + i + 1]);
        }
    }
}