using Aoc;
using static Aoc.Helpers;

namespace Aoc2022;

public class Solver202208 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var trees = new Grid2D<int>(lines.ToArray2D(ParseChar));
        var visible = trees.Spawn(false); 
        for (var row = 0; row < visible.Height; row++)
        {
            CheckVisibility(trees, visible, new Point(0, row), 1, 0);
            CheckVisibility(trees, visible, new Point(visible.Width - 1, row), -1, 0);
        }
        for (var col = 0; col < visible.Width; col++)
        {
            CheckVisibility(trees, visible, new Point(col, 0), 0, 1);
            CheckVisibility(trees, visible, new Point(col, visible.Height - 1), 0, -1);
        }

        return (
            visible.ValueEnumerable().Count(value => value == true),
            trees.CoordEnumerable().Select(tree => ComputeScenicScore(trees, tree)).Max()
        );
    }

    private static int ComputeScenicScore(Grid2D<int> trees, Point tree)
    {
        return ComputeDirectionScenicScore(trees, tree, 1, 0)
            * ComputeDirectionScenicScore(trees, tree, -1, 0)
            * ComputeDirectionScenicScore(trees, tree, 0, 1)
            * ComputeDirectionScenicScore(trees, tree, 0, -1);
    }

    private static int ComputeDirectionScenicScore(Grid2D<int> trees, Point tree, int dx, int dy)
    {
        return trees.EnumerateValues(tree, point => point.Delta(dx, dy)).Skip(1)
            .TakeWhileInclusive(height => height < trees[tree]).Count();
    }

    private static void CheckVisibility(Grid2D<int> trees, Grid2D<bool> visible, Point start, int dx, int dy)
    {
        var largest = trees[start];
        visible[start] = true;
        foreach (var current in visible.Enumerate(start.Delta(dx, dy), point => point.Delta(dx, dy)))
        {
            if (trees[current] > largest)
            {
                largest = trees[current];
                visible[current] = true;
            }
        }
    }
}