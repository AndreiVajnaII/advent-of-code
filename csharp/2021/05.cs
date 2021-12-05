public class Solver202105 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var ventLines = lines.Select(ParseLine);
        var horizontalOrVerticalLines = from line in ventLines
                                        where line.IsHorizontal || line.IsVertical
                                        select line;
        var intersectionPoints = horizontalOrVerticalLines.Pairwise()
                // .Where(pair => !pair.line1.Equals(pair.line2)))
            .SelectMany(IntersectLines);
        return intersectionPoints
            .Distinct().Count();
    }

    private Line ParseLine(string s)
    {
        var test = s.Split(" -> ").Select(ParsePoint).ToArray();
        var (a, b, _) = s.Split(" -> ").Select(ParsePoint);
        return new Line(a, b);
    }

    private Point ParsePoint(string s)
    {
        var (x, y, _) = s.Split(",").Select(int.Parse);
        return new Point(x, y);
    }

    private IEnumerable<Point> IntersectLines((Line line1, Line line2) vents)
    {
        if (vents.line1.IsVertical && vents.line2.IsHorizontal)
        {
            var intersection = new Point(vents.line1.A.X, vents.line2.A.Y);
            if (vents.line1.Includes(intersection) && vents.line2.Includes(intersection))
            {
                yield return intersection;
            }
        }
        else if (vents.line1.IsHorizontal && vents.line2.IsVertical)
        {
            var intersection = new Point(vents.line2.A.X, vents.line1.A.Y);
            if (vents.line1.Includes(intersection) && vents.line2.Includes(intersection))
            {
                yield return intersection;
            }
        }
        else if (vents.line1.IsHorizontal && vents.line2.IsHorizontal
            && vents.line1.A.Y == vents.line2.A.Y)
        {
            var points = new int[] { vents.line1.A.X, vents.line1.B.X, vents.line2.A.X, vents.line2.B.X };
            Array.Sort(points);
            if ((vents.line1.Includes(new Point(points[0], vents.line1.A.Y)) && vents.line2.Includes(new Point(points[1], vents.line1.A.Y)))
                || (vents.line2.Includes(new Point(points[0], vents.line1.A.Y)) && vents.line1.Includes(new Point(points[1], vents.line1.A.Y))))
            {
                for (int x = points[1]; x <= points[2]; x++)
                {
                    yield return new Point(x, vents.line1.A.Y);
                }
            }
        }
        else if (vents.line1.IsVertical && vents.line2.IsVertical
            && vents.line1.A.X == vents.line2.A.X)
        {
            var points = new int[] { vents.line1.A.Y, vents.line1.B.Y, vents.line2.A.Y, vents.line2.B.Y };
            Array.Sort(points);
            if ((vents.line1.Includes(new Point(vents.line1.A.X, points[0])) && vents.line2.Includes(new Point(vents.line1.A.X, points[1])))
                || (vents.line2.Includes(new Point(vents.line1.A.X, points[0])) && vents.line1.Includes(new Point(vents.line1.A.X, points[1]))))
            {
                for (int y = points[1]; y <= points[2]; y++)
                {
                    yield return new Point(vents.line1.A.X, y);
                }
            }
        }
    }
}

struct Point
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}

struct Line
{
    public Point A { get; private set; }
    public Point B { get; private set; }

    public Line(Point a, Point b)
    {
        A = a;
        B = b;
    }

    public override string ToString()
    {
        return $"{A} -> {B}";
    }

    public bool IsHorizontal
    {
        get
        {
            return A.Y == B.Y;
        }
    }

    public bool IsVertical
    {
        get
        {
            return A.X == B.X;
        }
    }

    public bool Includes(Point p)
    {
        return ((A.X <= p.X && p.X <= B.X) || (B.X <= p.X && p.X <= A.X))
            && ((A.Y <= p.Y && p.Y <= B.Y) || (B.Y <= p.Y && p.Y <= A.Y));
    }
}