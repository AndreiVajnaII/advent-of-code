public class Solver202120 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var algorithm = lines[0];
        var image = ParseGrid(lines.Skip(2));
        return (2.TimesIterate(image => Enhance(image, algorithm), image).Count,
            50.TimesIterate(image => Enhance(image, algorithm), image).Count);
    }

    private EnhancedImage Enhance(EnhancedImage image, string algorithm)
    {
        var newImage = new EnhancedImage();
        foreach (var p in Point.EnumeratePoints(image.Xmin - 1, image.Ymin - 1, image.Xmax + 1, image.Ymax + 1))
        {
            string binary = String.Join("", p.SelectAdjacents(Grid2D.allNeighbours).Append(p)
                                .OrderBy(adj => adj.Y).ThenBy(adj => adj.X).Select(image.ExtendedValueAt));
            var newPixel = algorithm[(int)Helpers.ParseBinary(
                binary)];
            newImage.SetPixel(p, newPixel);
        }
        newImage.OutOfBoundsValue = algorithm[(int)Helpers.ParseBinary(
                Enumerable.Repeat(image.OutOfBoundsValue, 9).AsString())];
        return newImage;
    }

    private EnhancedImage ParseGrid(IEnumerable<string> grid)
    {
        var image = new EnhancedImage();
        int y = 0;
        foreach (var line in grid)
        {
            for (int x = 0; x < line.Length; x++)
            {
                image.SetPixel(new Point(x, y), line[x]);
            }
            y++;
        }
        return image;
    }
}

public class EnhancedImage : SparseGrid<char>
{
    private char outOfBoundsValue = '0';
    public char OutOfBoundsValue
    {
        get { return outOfBoundsValue; }
        set { outOfBoundsValue = value == '#' ? '1' : '0'; }
    }
    
    public EnhancedImage() : base('0') { }

    public void SetPixel(Point p, char pixel)
    {
        if (pixel == '#')
        {
            Set(p, '1');
        }
    }

    public char ExtendedValueAt(Point p) {
        return IsInBounds(p) ? ValueAt(p) : OutOfBoundsValue;
    }
}