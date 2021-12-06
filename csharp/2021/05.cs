public class Solver202105 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var ventLines = lines.Select(ParseLine);
        return (CountDistinctIntersections(ventLines.Where(line => line.IsHorizontal || line.IsVertical)),
            CountDistinctIntersections(ventLines));
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

    private int CountDistinctIntersections(IEnumerable<Line> lines)
    {
        return lines.Pairwise().SelectMany(IntersectLines).Distinct().Count();
    }

    private IEnumerable<Point> IntersectLines((Line, Line) vents)
    {
        var (line1, line2) = vents;
        if (line1.IsVertical)
        {
            (line2, line1) = vents;
        }
        var xPoints = new int[] { line1.A.X, line1.B.X, line2.A.X, line2.B.X };
        Array.Sort(xPoints);
        var yPoints = new int[] { line1.A.Y, line1.B.Y, line2.A.Y, line2.B.Y };
        Array.Sort(yPoints);
        if (line1.IsVertical)
        {
            if (line1.A.X == line2.A.X)
            {
                var x = line1.A.X;
                if ((line1.Includes(new Point(x, yPoints[0])) && line2.Includes(new Point(x, yPoints[1])))
                    || (line2.Includes(new Point(x, yPoints[0])) && line1.Includes(new Point(x, yPoints[1]))))
                {
                    for (int y = yPoints[1]; y <= yPoints[2]; y++)
                    {
                        yield return new Point(x, y);
                    }
                }
            }
        }
        else
        {
            if (!line2.IsVertical && line1.Slope == line2.Slope)
            {
                if (line1.YIntercept == line2.YIntercept)
                {
                    if ((line1.Includes(new Point(xPoints[0], line1.CalculateY(xPoints[0]))) && line2.Includes(new Point(xPoints[1], line2.CalculateY(xPoints[1]))))
                        || (line2.Includes(new Point(xPoints[0], line2.CalculateY(xPoints[0]))) && line1.Includes(new Point(xPoints[1], line1.CalculateY(xPoints[1])))))
                    {
                        for (int x = xPoints[1]; x <= xPoints[2]; x++)
                        {
                            yield return new Point(x, line1.CalculateY(x));
                        }
                    }
                }
            }
            else
            {
                var x = line2.IsVertical ? line2.A.X : (line2.YIntercept - line1.YIntercept) / (line1.Slope - line2.Slope);
                var intersection = new Point(x, line1.CalculateY(x));
                if (line1.Includes(intersection) && line2.Includes(intersection))
                {
                    yield return intersection;
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

    public int Slope
    {
        get
        {
            return (A.Y - B.Y) / (A.X - B.X);
        }
    }

    public int YIntercept
    {
        get
        {
            return A.Y - Slope * A.X;
        }
    }

    public bool Includes(Point p)
    {
        return ((A.X <= p.X && p.X <= B.X) || (B.X <= p.X && p.X <= A.X))
            && ((A.Y <= p.Y && p.Y <= B.Y) || (B.Y <= p.Y && p.Y <= A.Y))
            && (IsVertical ? p.X == A.X : p.Y == CalculateY(p.X));
    }

    public int CalculateY(int x)
    {
        return Slope * x + YIntercept;
    }
}