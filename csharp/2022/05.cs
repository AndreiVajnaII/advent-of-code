using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Aoc;
using static Aoc.Helpers;

namespace Aoc2022;

public class Solver202205 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        using var linesEn = lines.AsEnumerable().GetEnumerator();
        linesEn.MoveNext();

        var stacks = ParseStacks(linesEn);
        var moves = ParseMoves(linesEn).ToImmutableArray();
        return (
            OperateCrane(stacks, moves, CrateMover9000),
            OperateCrane(stacks, moves, CrateMover9001)
        );
    }

    private static ImmutableArray<ImmutableArray<char>> ParseStacks(IEnumerator<string> lineEnumerator)
    {
        var line = lineEnumerator.Current;
        var stacks = InitializeImmutableArray<List<char>>(line.Length / 4 + 1);

        while (line[1] != '1')
        {
            for (var i = 0; i < stacks.Length; i++)
            {
                var c = line[i * 4 + 1];
                if (c != ' ')
                {
                    stacks[i].Insert(0, c);
                }
            }

            lineEnumerator.MoveNext();
            line = lineEnumerator.Current;
        }

        lineEnumerator.MoveNext();
        return stacks.Select(stack => stack.ToImmutableArray()).ToImmutableArray();
    }

    private static IEnumerable<(int, int, int)> ParseMoves(IEnumerator<string> linesEn)
    {
        var moveRegex = new Regex(@"move (\d+) from (\d+) to (\d+)");
        while (linesEn.MoveNext())
        {
            yield return moveRegex.Match(linesEn.Current).GroupValues().Select(int.Parse).AsTuple3();
        }
    }

    private static string OperateCrane(IEnumerable<ImmutableArray<char>> initialStacks,
        IEnumerable<(int count, int src, int dest)> moves,
        Action<ImmutableArray<Stack<char>>, int, int, int> crane)
    {
        var stacks = initialStacks.Select(stack => new Stack<char>(stack))
            .ToImmutableArray();
        foreach (var move in moves)
        {
            crane.Invoke(stacks, move.count, move.src, move.dest);
        }

        return string.Join("", stacks.Select(stack => stack.Peek()));
    }

    private static void CrateMover9000(ImmutableArray<Stack<char>> stacks, int count, int src, int dest)
    {
        for (var i = 0; i < count; i++)
        {
            var crate = stacks[src - 1].Pop();
            stacks[dest - 1].Push(crate);
        }
    }

    private static void CrateMover9001(ImmutableArray<Stack<char>> stacks, int count, int src, int dest)
    {
        var movedStack = new Stack<char>(count.Times(() => stacks[src - 1].Pop()));
        foreach (var crate in movedStack)
        {
            stacks[dest - 1].Push(crate);
        }
    }
}