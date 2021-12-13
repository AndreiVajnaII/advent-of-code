using System.Text.RegularExpressions;

public class Solver202113 : ISolver
{
    private readonly Regex foldRegex = new(@"fold along (x|y)=(\d+)", RegexOptions.Compiled);
    public dynamic Solve(string[] lines)
    {
        var (coordsString, folds) = Helpers.GroupLines(lines).AsTuple2();
        var coords = coordsString.Select(Point.Parse);
        return (Fold(coords, folds.First()).Count(),
            Environment.NewLine + Stringify(folds.Aggregate(coords, Fold)));
    }

    private string Stringify(IEnumerable<Point> points)
    {
        int width = points.Select(p => p.X).Max() + 1;
        int height = points.Select(p => p.Y).Max() + 1;
        var paper = new Grid2D<char>(width, height, ' ');
        foreach (var p in points)
        {
            paper.SetValueAt(p, '#');
        }
        return paper.ToString();
    }

    private IEnumerable<Point> Fold(IEnumerable<Point> coords, string fold)
    {
        var m = foldRegex.Match(fold);
        string axis = m.Groups[1].Value;
        int mirror = int.Parse(m.Groups[2].Value);
        return coords.Select(coord => Fold(coord, axis, mirror)).Distinct();
    }

    private Point Fold(Point coord, string axis, int mirror)
    {
        return axis == "x"
            ? new Point(Fold(coord.X, mirror), coord.Y)
            : new Point(coord.X, Fold(coord.Y, mirror));
    }

    private int Fold(int value, int mirror)
    {
        return value <= mirror ? value : 2 * mirror - value;
    }
}
