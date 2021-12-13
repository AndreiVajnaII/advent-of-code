public static class Helpers
{
    public static string PadDay(string day)
    {
        return day.Length < 2 ? $"0{day}" : day;
    }

    public static uint ParseBinary(string s)
    {
        return Convert.ToUInt32(s, 2);
    }

    public static int ParseChar(char c)
    {
        return (int)(c - '0');
    }

    // group lines separated by blank lines
    public static IEnumerable<IEnumerable<string>> GroupLines(string[] lines)
    {
        bool newGroup = true;
        var group = new List<string>();
        foreach (var line in lines)
        {
            if (line == "")
            {
                yield return group;
                group = new List<string>();
                newGroup = true;
            }
            else
            {
                group.Add(line);
                if (newGroup)
                {
                    newGroup = false;
                }
            }
        }
        yield return group;
    }
}

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        return enumerable.SelectMany(x => x);
    }

    public static IEnumerable<(T, T)> Pairwise<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.SelectMany((item1, i) => enumerable.Skip(i + 1).Select(item2 => (item1, item2)));
    }

    public static void Deconstruct<T>(this IEnumerable<T> enumerable, out T first, out IEnumerable<T> rest)
    {
        first = enumerable.First();
        rest = enumerable.Skip(1);
    }

    public static void Deconstruct<T>(this IEnumerable<T> enumerable, out T first, out T second, out IEnumerable<T> rest)
    {
        (first, var tail) = enumerable;
        (second, rest) = tail;
    }

    public static (T, T) AsTuple2<T>(this IEnumerable<T> enumerable)
    {
        var (first, second, _) = enumerable;
        return (first, second);
    }

    public static T[,] ToArray2D<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        var array = enumerable.Select(e => e.ToArray()).ToArray();
        T[,] array2D = new T[array.Length, array[0].Length];
        for (int row = 0; row < array2D.GetLength(0); row++)
        {
            for (int col = 0; col < array2D.GetLength(1); col++)
            {
                array2D[row, col] = array[row][col];
            }
        }
        return array2D;
    }
}

public class Grid2D
{
    public static readonly (int X, int Y)[] orthogonalNeighbours = new[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
    public static readonly (int X, int Y)[] diagonalNeighbours = new[] { (1, 1), (1, -1), (-1, 1), (-1, -1) };
    public static readonly (int X, int Y)[] allNeighbours = orthogonalNeighbours.Union(diagonalNeighbours).ToArray();
}

public class Grid2D<T>
{
    private T[,] grid;

    public int Width
    {
        get
        {
            return grid.GetLength(1);
        }
    }

    public int Height
    {
        get
        {
            return grid.GetLength(0);
        }
    }

    public int Count
    {
        get
        {
            return Width * Height;
        }
    }

    public Grid2D(T[,] grid)
    {
        this.grid = grid;
    }

    public Grid2D(int width, int height, T initialValue)
    {
        this.grid = new T[height, width];
        foreach (var p in CoordEnumerable())
        {
            SetValueAt(p, initialValue);
        }
    }

    public T ValueAt(Point p) => grid[p.Y, p.X];

    public void SetValueAt(Point p, T value)
    {
        grid[p.Y, p.X] = value;
    }

    public IEnumerable<T> ValueEnumerable() => CoordEnumerable().Select(ValueAt);

    public IEnumerable<Point> CoordEnumerable() => GridEnumerable().Flatten();

    public IEnumerable<IEnumerable<T>> GridValueEnumerable()
        => GridEnumerable().Select(line => line.Select(ValueAt));

    public IEnumerable<IEnumerable<Point>> GridEnumerable()
        => Enumerable.Range(0, Height).Select(LineEnumerable);

    public IEnumerable<Point> LineEnumerable(int y)
        => Enumerable.Range(0, Width).Select(x => new Point(x, y));

    public override string ToString()
        => String.Join(Environment.NewLine, GridValueEnumerable()
            .Select(line => String.Join(' ', line)));

    public IEnumerable<T> Adjacents(Point p, (int X, int Y)[] neighbours)
        => AdjacentPoints(p, neighbours).Select(ValueAt);

    public IEnumerable<Point> AdjacentPoints(Point p, (int X, int Y)[] neighbours)
        => neighbours.Select(d => new Point(p.X + d.X, p.Y + d.Y)).Where(IsInBounds);

    public bool IsInBounds(Point p)
        => grid.GetLowerBound(0) <= p.Y && p.Y <= grid.GetUpperBound(0)
        && grid.GetLowerBound(1) <= p.X && p.X <= grid.GetUpperBound(1);

}

public struct Point
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Point Parse(string s)
    {
        var (x, y) = s.Split(',').Select(int.Parse).AsTuple2();
        return new Point(x, y);
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}

public class Graph<T> : Dictionary<T, ISet<T>> where T : notnull
{
    public void AddTwoWay(T start, T end)
    {
        AddOneWay(start, end);
        AddOneWay(end, start);
    }

    public void AddOneWay(T start, T end)
    {
        if (!ContainsKey(start))
        {
            Add(start, new HashSet<T>());
        }
        this[start].Add(end);
    }

    public TVisitor Traverse<TVisitor>(T current, T end, TVisitor visitor) where TVisitor : GraphVisitor<T>
    {
        if (current.Equals(end))
        {
            visitor.End(end);
        }
        else
        {
            visitor.Visit(current);
            foreach (var neighbour in this[current])
            {
                if (!visitor.HasVisited(neighbour))
                {
                    Traverse(neighbour, end, visitor);
                }
            }
            visitor.Unvisit(current);
        }
        return visitor;
    }
}

public class GraphVisitor<T>
{
    private IGraphVisitPolicy<T> visitPolicy;

    public GraphVisitor() : this(new DefaultGraphVisitPolicy<T>()) { }

    public GraphVisitor(IGraphVisitPolicy<T> visitPolicy)
    {
        this.visitPolicy = visitPolicy;
    }

    public virtual void Visit(T node)
    {
        visitPolicy.Visit(node);
    }

    public virtual void End(T node) { }

    public virtual bool HasVisited(T node)
    {
        return visitPolicy.HasCompletelyVisited(node);
    }

    public virtual void Unvisit(T node)
    {
        visitPolicy.Unvisit(node);
    }
}

public interface IGraphVisitPolicy<T>
{
    void Visit(T node);
    void Unvisit(T node);
    bool HasCompletelyVisited(T node);
}

public class DefaultGraphVisitPolicy<T> : IGraphVisitPolicy<T>
{
    private ISet<T> visited = new HashSet<T>();
    public virtual bool HasCompletelyVisited(T node)
    {
        return visited.Contains(node);
    }

    public virtual void Visit(T node)
    {
        visited.Add(node);
    }

    public virtual void Unvisit(T node)
    {
        visited.Remove(node);
    }

}