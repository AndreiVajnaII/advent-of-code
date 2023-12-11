using Aoc;

namespace Aoc2023;

public class Solver202311 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var cosmos = new Grid2D<char>(lines.ToArray2D());
        var expandedRows = cosmos.GridValueEnumerable().IndexesOf(row => row.All(cell => cell == '.')).ToArray();
        var expandedCols = cosmos.ColumnEnumerable().IndexesOf(col => col.All(p => cosmos[p] == '.')).ToArray();
        var galaxyPairs = cosmos.CoordEnumerable()
            .Where(point => cosmos[point] == '#')
            .Pairwise().ToArray();
        return (
            DistancesSum(galaxyPairs, expandedRows, expandedCols, 2),
            DistancesSum(galaxyPairs, expandedRows, expandedCols, 1_000_000)
        );
    }

    private static long DistancesSum((Point, Point)[] galaxyPairs, int[] expandedRows, int[] expandedCols, int expansion)
        => galaxyPairs
            .Select(pair => Distance(pair.Item1, pair.Item2, expandedRows, expandedCols, expansion))
            .Sum();

    private static long Distance(Point p1, Point p2, int[] expandedRows, int[] expandedCols, long expansion)
        => p1.ManhattanDistance(p2)
                + expandedRows.Count(index => index.IsBetween(p1.Y, p2.Y)) * (expansion - 1)
                + expandedCols.Count(index => index.IsBetween(p1.X, p2.X)) * (expansion - 1);
}