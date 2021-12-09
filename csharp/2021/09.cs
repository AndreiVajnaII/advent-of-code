public class Solver202109 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var grid = new Grid2D<int>(lines.Select(line =>
            line.AsEnumerable().Select(Helpers.ParseChar)).ToArray2D());
        return (from p in grid.CoordEnumerable()
            where grid.Adjacents(p).All(adjValue => grid.ValueAt(p) < adjValue)
            select grid.ValueAt(p) + 1).Sum();
    }
}